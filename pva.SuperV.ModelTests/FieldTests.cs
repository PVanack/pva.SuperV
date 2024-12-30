using pva.SuperV.Model;
using pva.SuperV.Model.Exceptions;

namespace pva.SuperV.ModelTests
{
    public class FieldTests
    {
        [Fact]
        public void GivenInvalidFieldName_WhenCreatingField_ThenInvalidFieldNameExceptionIsThrown()
        {
            // WHEN/THEN
            Assert.Throws<InvalidFieldNameException>(() => new FieldDefinition<int>("AZ.0", 10));
        }
    }
}