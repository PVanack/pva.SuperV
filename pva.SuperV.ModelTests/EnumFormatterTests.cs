using FluentAssertions;
using pva.SuperV.Model;
using pva.SuperV.Model.Exceptions;

namespace pva.SuperV.ModelTests
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
            formatter.Should().NotBeNull();
            formatter.Values.Should()
                .HaveCount(2).And
                .Contain(
                    new KeyValuePair<int, string>(0, "Closed"),
                    new KeyValuePair<int, string>(1, "Opened")
                    );
        }

        [Fact]
        public void WhenCreatingEnumFormatterWithValuesAndDescriptions_ThenFormatterIsCreated()
        {
            // WHEN
            EnumFormatter formatter = new(EnumName, new Dictionary<int, string>() { { 23, "Closed" }, { 32, "Opened" } });

            // THEN
            formatter.Should().NotBeNull();
            formatter.AllowedTypes.Should()
                .HaveCount(2).And
                .Contain([typeof(int), typeof(long)]);
            formatter.Values.Should()
                .HaveCount(2).And
                .Contain(
                    new KeyValuePair<int, string>(23, "Closed"),
                    new KeyValuePair<int, string>(32, "Opened")
                    );
        }

        [Theory]
        [InlineData("AZ.0")]
        [InlineData("0AZ")]
        public void WhenCreatingEnumFormatterWithInvalidName_ThenExceptionIsThrown(string invalidName)
        {
            // WHEN
            var exception = Assert.Throws<InvalidFormatterNameException>(() => new EnumFormatter(invalidName, new Dictionary<int, string>() { { 23, "Closed" }, { 32, "Opened" } }));

            // THEN
            Assert.IsType<InvalidFormatterNameException>(exception);
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
            stringValue.Should()
                .NotBeNull().And
                .Be("Closed");
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
            stringValue.Should()
                .NotBeNull().And
                .Be("0 ?");
        }
    }
}