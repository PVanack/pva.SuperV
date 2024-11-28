using pva.SuperVAccess.Exceptions;
using System.Reflection;
using System.Reflection.Emit;

namespace pva.SuperVAccess
{
    public class Class(String className, TypeBuilder typeBuilder)
    {
        //TODO Validate class name with regex
        public TypeBuilder? TypeBuilder { get; } = typeBuilder;
        /// <summary>Gets or sets the name of the class.</summary>
        public String Name { get; set; } = className;
        /// <summary>Gets the fields defining the class.</summary>
        public Dictionary<String, dynamic> Fields { get; init; } = new Dictionary<String, dynamic>(StringComparer.OrdinalIgnoreCase);

        public Field<T> AddField<T>(Field<T> field)
        {
            if (Fields.ContainsKey(field.Name))
            {
                throw new FieldAlreadyExistException(field.Name);
            }

            //CreateFieldInClass(field);
            Fields.Add(field.Name, field);
            return field;
        }

        //private void CreateFieldInClass<T>(Field<T> field)
        //{
        //    // Define a private String field named "DynamicField" in the type.
        //    FieldBuilder fldBuilder = TypeBuilder?.DefineField(field.Name,
        //        field.Value?.GetType(), FieldAttributes.Private | FieldAttributes.Static);
        //    // Create the constructor.
        //    Type[] constructorArgs = { typeof(String) };
        //    ConstructorBuilder constructor = TypeBuilder?.DefineConstructor(
        //       MethodAttributes.Public, CallingConventions.Standard, constructorArgs);
        //    ILGenerator constructorIL = constructor.GetILGenerator();
        //    constructorIL.Emit(OpCodes.Ldarg_0);
        //    ConstructorInfo? superConstructor = typeof(Object).GetConstructor(new Type[0]);
        //    constructorIL.Emit(OpCodes.Call, superConstructor!);
        //    constructorIL.Emit(OpCodes.Ldarg_0);
        //    constructorIL.Emit(OpCodes.Ldarg_1);
        //    constructorIL.Emit(OpCodes.Stfld, fldBuilder);
        //    constructorIL.Emit(OpCodes.Ret);

        //    // Create the Getter method.
        //    MethodBuilder methBuilder = TypeBuilder.DefineMethod($"Get{field.Name}",
        //                         MethodAttributes.Public, field.Value?.GetType(), null);
        //    ILGenerator methodIL = methBuilder.GetILGenerator();
        //    methodIL.Emit(OpCodes.Ldarg_0);
        //    methodIL.Emit(OpCodes.Ldfld, fldBuilder);
        //    methodIL.Emit(OpCodes.Ret);

        //    var buildedField = TypeBuilder.CreateType();
        //}
    }
}
