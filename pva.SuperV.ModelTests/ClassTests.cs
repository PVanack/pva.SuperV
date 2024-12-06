using FluentAssertions;
using pva.SuperV.Model;
using pva.SuperV.Model.Exceptions;

namespace pva.SuperV.ModelTests
{
    public class ClassTests
    {
        private const string PROJECT_NAME = "TestProject";
        private const string CLASS_NAME = "TestClass";

        [Fact]
        public void GivenInvalidClassName_WhenCreatingClass_ThenInvalidClassNameExcpetionIsThrown()
        {
            // WHEN/THEN
            Assert.Throws<InvalidClassNameException>(() => new Class("AZ.0"));
        }


        [Fact]
        public void GivenEmptyProject_WhenCreatingClass_ThenClassIsCreatedWithNoField()
        {
            // GIVEN
            WipProject project = Project.CreateProject(PROJECT_NAME);

            // WHEN
            Class clazz = project.AddClass(CLASS_NAME);

            // THEN
            clazz.Should().NotBeNull();
            clazz.Name.Should().Be(CLASS_NAME);
            clazz.FieldDefinitions.Should().NotBeNull();
            clazz.FieldDefinitions.Should().BeEmpty();
        }
    }
}
