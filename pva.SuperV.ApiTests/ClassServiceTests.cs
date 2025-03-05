using pva.SuperV.Api.Services.Classes;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.EngineTests;
using pva.SuperV.Model.Classes;
using Shouldly;

namespace pva.SuperV.ApiTests
{
    [Collection("Project building")]
    public class ClassServiceTests
    {
        private readonly ClassService _classService;
        private readonly RunnableProject runnableProject;
        private readonly WipProject wipProject;

        public ClassServiceTests()
        {
            _classService = new();
            runnableProject = ProjectHelpers.CreateRunnableProject();
            wipProject = ProjectHelpers.CreateWipProject(null);
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

        [Fact]
        public void CreateClassInWipProject_ShouldCreateClass()
        {
            ClassModel expectedClass = new("NewClass", null);
            // Act & Assert
            ClassModel createClassModel = _classService.CreateClass(wipProject.GetId(), expectedClass);

            createClassModel.ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedClass);
        }

        [Fact]
        public void DeleteClassInWipProject_ShouldDeleteClass()
        {
            ClassModel expectedClass = new("NewClass", null);
            // Act
            _classService.DeleteClass(wipProject.GetId(), expectedClass.Name);

            // Assert
            wipProject.Classes.ShouldNotContainKey(expectedClass.Name);
        }
    }
}
