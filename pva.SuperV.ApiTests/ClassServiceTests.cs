using pva.SuperV.Api;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.EngineTests;
using Shouldly;

namespace pva.SuperV.ApiTests
{
    [Collection("Project building")]
    public class ClassServiceTests
    {
        private readonly ClassService _classService;
        private readonly RunnableProject runnableProject;

        public ClassServiceTests()
        {
            _classService = new();
            runnableProject = ProjectHelpers.CreateRunnableProject();
        }

        [Fact]
        public void GetClasses_ShouldReturnListOfClasses()
        {
            // Act
            var result = _classService.GetClasses(runnableProject.GetId());

            // Assert
            result.Count.ShouldBe(2);
            result.ShouldContain(c => c.Name == ProjectHelpers.ClassName);
            result.ShouldContain(c => c.Name == ProjectHelpers.BaseClassName);
        }

        [Fact]
        public void GetClass_ShouldReturnClass_WhenClassExists()
        {
            // Act
            var result = _classService.GetClass(runnableProject.GetId(), ProjectHelpers.ClassName);

            // Assert
            result.ShouldNotBeNull();
            result.Name.ShouldBe(ProjectHelpers.ClassName);
        }

        [Fact]
        public void GetClass_ShouldThrowUnknownEntityException_WhenClassDoesNotExist()
        {
            // Act & Assert
            Assert.Throws<UnknownEntityException>(()
                => _classService.GetClass(runnableProject.GetId(), "UnknownClass"));
        }
    }
}
