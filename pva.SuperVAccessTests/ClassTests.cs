using FluentAssertions;
using pva.SuperVAccess;

namespace pva.SuperVAccessTests
{
    public class ClassTests
    {
        private const string PROJECT_NAME = "TestProject";
        private const string CLASS_NAME = "TestClass";

        [Fact]
        public void GivenEmptyProject_WhenCreatingClass_ThenClassIsCreatedWithNoField()
        {
            // GIVEN
            Project project = Project.CreateProject(PROJECT_NAME);

            // WHEN
            Class clazz = project.AddClass(CLASS_NAME);

            // THEN
            clazz.Should().NotBeNull();
            clazz.Name.Should().Be(CLASS_NAME);
            clazz.Fields.Should().NotBeNull();
            clazz.Fields.Should().BeEmpty();
        }
    }
}
