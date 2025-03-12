using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.FieldValueFormatters;
using Shouldly;

namespace pva.SuperV.EngineTests
{
    public class EnumFormatterTests
    {
        private const string EnumName = "ClosedOpened";

        [Fact]
        public void WhenCreatingEnumFormatterWithOnlyDescriptions_ThenFormatterIsCreatedWithAppropriateKeys()
        {
            // WHEN
            EnumFormatter formatter = new(EnumName, ["Closed", "Opened"]);

            // THEN
            formatter.Values.ShouldNotBeNull()
                .ShouldContain(valuePair =>
                    (valuePair.Key == 0 && valuePair.Value.Equals("Closed")) ||
                    (valuePair.Key == 1 && valuePair.Value.Equals("Opened")));
        }

        [Fact]
        public void WhenCreatingEnumFormatterWithValuesAndDescriptions_ThenFormatterIsCreated()
        {
            // WHEN
            EnumFormatter formatter = new(EnumName, new Dictionary<int, string>() { { 23, "Closed" }, { 32, "Opened" } });

            // THEN
            formatter.AllowedTypes.ShouldNotBeNull()
                .ShouldContain(type =>
                    type == typeof(int) ||
                    type == typeof(long));
            formatter.Values.ShouldNotBeNull()
                .ShouldContain(valuePair =>
                    (valuePair.Key == 23 && valuePair.Value.Equals("Closed")) ||
                    (valuePair.Key == 32 && valuePair.Value.Equals("Opened")));
        }

        [Theory]
        [InlineData("AZ.0")]
        [InlineData("0AZ")]
        public void WhenCreatingEnumFormatterWithInvalidName_ThenExceptionIsThrown(string invalidName)
        {
            // WHEN
            Assert.Throws<InvalidIdentifierNameException>(() => new EnumFormatter(invalidName, new Dictionary<int, string>() { { 23, "Closed" }, { 32, "Opened" } }));
        }

        [Fact]
        public void GivenFormatter_WhenFormattingValueInValues_ThenAssociatedStringValueIsReturned()
        {
            // GIVEN
            EnumFormatter formatter = new(EnumName, new Dictionary<int, string>() { { 23, "Closed" }, { 32, "Opened" } });

            // WHEN
            const int value = 23;
            string? stringValue = formatter.ConvertToString(value);

            // THEN
            stringValue.ShouldNotBeNull()
                .ShouldBe("Closed");
        }

        [Fact]
        public void GivenFormatter_WhenFormattingValueNotInValues_ThenValueWithQuestionMarkIsReturned()
        {
            // GIVEN
            EnumFormatter formatter = new(EnumName, new Dictionary<int, string>() { { 23, "Closed" }, { 32, "Opened" } });

            // WHEN
            const int value = 0;
            string? stringValue = formatter.ConvertToString(value);

            // THEN
            stringValue.ShouldNotBeNull()
                .ShouldBe("0 ?");
        }
    }
}