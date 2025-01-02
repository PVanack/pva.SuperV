using pva.SuperV.Model;
using pva.SuperV.Model.Exceptions;

namespace pva.SuperV.ModelTests
{
    public class FieldTests
    {
        [Theory]
        [InlineData("AS.0")]
        [InlineData("0AS")]
        [InlineData("AS-0")]
        public void GivenInvalidFieldName_WhenCreatingField_ThenInvalidFieldNameExceptionIsThrown(string invalidFieldName)
        {
            // WHEN/THEN
            Assert.Throws<InvalidFieldNameException>(() => new FieldDefinition<int>(invalidFieldName, 10));
        }
    }
}