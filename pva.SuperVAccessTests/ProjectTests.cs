using FluentAssertions;
using pva.SuperVAccess;
using pva.SuperVAccess.Exceptions;

namespace pva.SuperVAccessTests
{
    public class ProjectTests
    {
        private const string PROJECT_NAME = "TestProject";
        private const string CLASS_NAME = "TestClass";

        [Fact]
        public void GivenNoCurrentProject_WhenCreatingProject_ThenEmptyProjectIsCreated()
        {
            // WHEN
            Project project = Project.CreateProject(PROJECT_NAME);

            // THEN
            project.Should().NotBeNull();
            Project.CurrentProject.Should().NotBeNull();
            project.Name.Should().Be(PROJECT_NAME);
            project.AssemblyBuilder.Should().NotBeNull();
            project.ModuleBuilder.Should().NotBeNull();
            project.Classes.Should().BeEmpty();
        }

        [Fact]
        public void GivenNewProject_WhenAddingClassToProject_ThenClassIsAdded()
        {
            // GIVEN
            Project project = Project.CreateProject(PROJECT_NAME);

            // WHEN
            project.AddClass(CLASS_NAME);

            // THEN
            project.Classes.Should().Satisfy(entry => entry.Key.Equals(CLASS_NAME));
            project.Classes[CLASS_NAME].Should().NotBeNull();
        }

        [Fact]
        public void GivenNewProject_WhenAddingTwiceClassToProject_ThenClassAlreadyExistExceptionIsThrown()
        {
            // GIVEN
            Project project = Project.CreateProject(PROJECT_NAME);

            // WHEN
            project.AddClass(CLASS_NAME);
            project.Classes.Should().Satisfy(entry => entry.Key.Equals(CLASS_NAME));
            project.Classes[CLASS_NAME].Should().NotBeNull();
            var exception = Assert.Throws<ClassAlreadyExistException>(() => project.AddClass(CLASS_NAME));

            // THEN
            Assert.IsType<ClassAlreadyExistException>(exception);
        }

        [Fact]
        public void GivenNewProjectWithClass_WhenFindingClassInProject_ThenClassIsReturned()
        {
            // GIVEN
            Project project = Project.CreateProject(PROJECT_NAME);
            project.AddClass(CLASS_NAME);

            // WHEN
            Class? clazz = project.FindClass(CLASS_NAME);

            // THEN
            clazz.Should().NotBeNull();
        }

        [Fact]
        public void GivenNewProjectWithClass_WhenGettingClassInProject_ThenClassIsReturned()
        {
            // GIVEN
            Project project = Project.CreateProject(PROJECT_NAME);
            project.AddClass(CLASS_NAME);

            // WHEN
            Class clazz = project.GetClass(CLASS_NAME);

            // THEN
            clazz.Should().NotBeNull();
        }

        [Fact]
        public void GivenNewProject_WhenFindingClassInProject_ThenNullableIsReturned()
        {
            // GIVEN
            Project project = Project.CreateProject(PROJECT_NAME);

            // WHEN
            Class? clazz = project.FindClass(CLASS_NAME);

            // THEN
            clazz.Should().BeNull();
        }

        [Fact]
        public void GivenNewProject_WhenGettingClassInProject_ThenUnknownClassExceptionIsThrown()
        {
            // GIVEN
            Project project = Project.CreateProject(PROJECT_NAME);

            // WHEN
            var exception = Assert.Throws<UnknownClassException>(() => project.GetClass(CLASS_NAME));

            // THEN
            Assert.IsType<UnknownClassException>(exception);
        }
    }
}