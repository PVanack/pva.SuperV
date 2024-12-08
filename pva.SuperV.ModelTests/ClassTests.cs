using FluentAssertions;
using pva.SuperV.Model;
using pva.SuperV.Model.Exceptions;

namespace pva.SuperV.ModelTests
{
    public class ClassTests
    {
        private const string ProjectName = "TestProject";
        private const string ClassName = "TestClass";

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
            WipProject project = Project.CreateProject(ProjectName);

            // WHEN
            Class clazz = project.AddClass(ClassName);

            // THEN
            clazz.Should().NotBeNull();
            clazz.Name.Should().Be(ClassName);
            clazz.FieldDefinitions.Should().NotBeNull();
            clazz.FieldDefinitions.Should().BeEmpty();
        }
    }
}
