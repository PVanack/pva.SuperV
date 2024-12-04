using FluentAssertions;
using pva.SuperV.Model;
using pva.SuperV.Model.Exceptions;

namespace pva.SuperV.ModelTests
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

        [Fact]
        public void GivenProjectWithClassAndField_WhenGettingCode_ThenGeneratedCodeIsAsExpected()
        {
            // GIVEN
            Project project = Project.CreateProject(PROJECT_NAME);
            Class clazz = project.AddClass(CLASS_NAME);
            clazz.AddField(new Field<int>("IntField", 10));

            // WHEN
            String projectCode = project.GetCode();

            // THEN
            String expectedCode = $@"namespace {PROJECT_NAME} {{
public class {CLASS_NAME} {{
public System.String Name {{ get; set; }}
public System.Int32 IntField {{ get; set; }} = 10;

}}

}}
";
            projectCode.Should().BeEquivalentTo(expectedCode);
        }
    }
}