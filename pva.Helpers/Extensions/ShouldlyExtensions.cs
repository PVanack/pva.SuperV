using Shouldly;
using System.Collections;
using System.Linq.Expressions;

namespace pva.Helpers.Extensions;

public class EquivalencyOptions<T>
{
    public HashSet<string> ExcludedProperties { get; } = [];
    public HashSet<Type> ExcludedTypes { get; } = [];
    public TimeSpan? DateTimeTolerance { get; private set; }

    public EquivalencyOptions<T> Exclude(Expression<Func<T, object>> propertyExpression)
    {
        ArgumentNullException.ThrowIfNull(propertyExpression);

        ExcludedProperties.Add(GetPropertyPath(propertyExpression));
        return this;
    }

    public EquivalencyOptions<T> ExcludeType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        ExcludedTypes.Add(type);
        return this;
    }

    public EquivalencyOptions<T> UsingDateTimeTolerance(TimeSpan tolerance)
    {
        DateTimeTolerance = tolerance;
        return this;
    }

    private static string GetPropertyPath(Expression expression)
    {
        var propertyNames = new List<string>();
        ExtractPropertyPath(expression, propertyNames);
        propertyNames.Reverse();
        return string.Join(".", propertyNames);
    }

    private static void ExtractPropertyPath(Expression expression, List<string> propertyNames)
    {
        switch (expression)
        {
            case MemberExpression memberExpression:
                propertyNames.Add(memberExpression.Member.Name);
                ExtractPropertyPath(memberExpression.Expression!, propertyNames);
                break;
            case MethodCallExpression methodCallExpression:
                foreach (var argument in methodCallExpression.Arguments.Reverse())
                {
                    ExtractPropertyPath(argument, propertyNames);
                }
                break;
            case UnaryExpression unaryExpression:
                ExtractPropertyPath(unaryExpression.Operand, propertyNames);
                break;
            case LambdaExpression lambdaExpression:
                ExtractPropertyPath(lambdaExpression.Body, propertyNames);
                break;
            case ParameterExpression:
                break;
            default:
                throw new ArgumentException("Invalid property expression");
        }
    }
}


[ShouldlyMethods]
public static class ShouldlyExtensions
{
    public static void ShouldBeEquivalentTo<T>(this T? actual, T? expected, Action<EquivalencyOptions<T>> options) where T : class
    {
        if (typeof(IEnumerable).IsAssignableFrom(typeof(T)) && typeof(T) != typeof(string))
        {
            throw new InvalidOperationException("This overload is not intended for collection types.");
        }

        var equivalencyOptions = ValidateAndSetupOptions(actual, expected, options);

        CompareObjects(actual, expected, equivalencyOptions, new HashSet<(object, object)>());
    }

    public static void ShouldBeEquivalentTo<T>(this IEnumerable<T?> actual, IEnumerable<T?> expected, Action<EquivalencyOptions<T>> options)
    {
        var equivalencyOptions = ValidateAndSetupOptions(actual, expected, options);
        var actualList = actual.ToList();
        var expectedList = expected.ToList();

        actualList.Count.ShouldBe(expectedList.Count, "Collection counts do not match.");

        for (int i = 0; i < actualList.Count; i++)
        {
            CompareObjects(actualList[i], expectedList[i], equivalencyOptions, new HashSet<(object, object)>());
        }
    }

    private static EquivalencyOptions<T> ValidateAndSetupOptions<T>(object? actual, object? expected, Action<EquivalencyOptions<T>> options)
    {
        if (actual == null && expected != null || actual != null && expected == null)
        {
            throw new ShouldAssertException(@$"Comparing object equivalence:
actual: {actual?.GetType().ToString() ?? "null"}
expected: {expected?.GetType().ToString() ?? "null"}");
        }

        var equivalencyOptions = new EquivalencyOptions<T>();
        options(equivalencyOptions);

        return equivalencyOptions;
    }

    private static void CompareObjects<T>(object? actual, object? expected, EquivalencyOptions<T> options, HashSet<(object, object)> visitedObjects, string parentPath = "")
    {
        if (actual == null && expected == null)
        {
            return;
        }

        if (actual == null || expected == null)
        {
            throw new ShouldAssertException(@$"Comparing object equivalence, at path '{parentPath}':
actual: {actual?.GetType().ToString() ?? "null"}
expected: {expected?.GetType().ToString() ?? "null"}");
        }

        if (visitedObjects.Contains((actual, expected)))
        {
            return;
        }
        visitedObjects.Add((actual, expected));

        foreach (var property in actual.GetType().GetProperties())
        {
            var propertyPath = string.IsNullOrEmpty(parentPath) ? property.Name : $"{parentPath}.{property.Name}";
            if (property.GetIndexParameters().Length > 0)
            {
                continue; // Skip indexers
            }
            object? actualPropertyValue = property.GetValue(actual, null);
            if (options.ExcludedProperties.Contains(propertyPath)
                || (actualPropertyValue != null && IsOfType(options, actualPropertyValue)))
            {
                continue;
            }

            ComparePropertyValues(property.GetValue(actual, null), property.GetValue(expected, null), property, options, visitedObjects, propertyPath);
        }
    }

    private static bool IsOfType<T>(EquivalencyOptions<T> options, object actualPropertyValue)
    {
        return options.ExcludedTypes.Any(type => actualPropertyValue.GetType().IsSubclassOf(type));
    }

    private static void ComparePropertyValues<T>(object? actualValue, object? expectedValue, System.Reflection.PropertyInfo property, EquivalencyOptions<T> options, HashSet<(object, object)> visitedObjects, string propertyPath)
    {
        var hasNull = actualValue == null || expectedValue == null;
        if (!hasNull && property.PropertyType == typeof(DateTime) && options.DateTimeTolerance.HasValue)
        {
            ((DateTime)actualValue!).ShouldBe((DateTime)expectedValue!, options.DateTimeTolerance.Value, $"Property {propertyPath} does not match.");
        }
        else if (!hasNull && property.PropertyType == typeof(DateTimeOffset) && options.DateTimeTolerance.HasValue)
        {
            ((DateTimeOffset)actualValue!).ShouldBe((DateTimeOffset)expectedValue!, options.DateTimeTolerance.Value, $"Property {propertyPath} does not match.");
        }
        else if (!hasNull && IsCollectionType(property.PropertyType))
        {
            CompareCollections(actualValue!, expectedValue!, options, visitedObjects, propertyPath);
        }
        else if (!hasNull && actualValue.GetType().IsTypeDefinition)
        {
            actualValue.ShouldBeEquivalentTo(expectedValue, $"Property {propertyPath} does not match.");
        }
        else if (!hasNull && IsComplexType(actualValue!))
        {
            CompareObjects(actualValue, expectedValue, options, visitedObjects, propertyPath);
        }
        else
        {
            actualValue.ShouldBe(expectedValue, $"Property {propertyPath} does not match.");
        }
    }

    private static void CompareCollections<T>(object actual, object expected, EquivalencyOptions<T> options, HashSet<(object, object)> visitedObjects, string parentPath = "")
    {
        var actualList = ((IEnumerable<object>)actual).ToList();
        var expectedList = ((IEnumerable<object>)expected).ToList();

        actualList.Count.ShouldBe(expectedList.Count, "Collection counts do not match.");

        for (int i = 0; i < actualList.Count; i++)
        {
            CompareObjects(actualList[i], expectedList[i], options, visitedObjects, parentPath);
        }
    }

    private static bool IsCollectionType(Type type)
    {
        return typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);
    }

    private static bool IsComplexType(object obj)
    {
        var type = obj.GetType();
        return !type.IsPrimitive
            && !type.IsEnum
            && type != typeof(string)
            && type != typeof(decimal)
            && type != typeof(DateTime)
            && type != typeof(DateTimeOffset);
    }
}