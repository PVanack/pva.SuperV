using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;

namespace pva.SuperV.EngineTests
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