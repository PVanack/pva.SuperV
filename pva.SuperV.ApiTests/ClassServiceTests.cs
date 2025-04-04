﻿using pva.SuperV.Api.Services.Classes;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.EngineTests;
using pva.SuperV.Model.Classes;
using Shouldly;

namespace pva.SuperV.ApiTests
{
    [Collection("Project building")]
    public class ClassServiceTests : SuperVTestsBase
    {
        private readonly ClassService classService;
        private readonly RunnableProject runnableProject;
        private readonly WipProject wipProject;

        public ClassServiceTests()
        {
            classService = new();
            runnableProject = CreateRunnableProject();
            wipProject = CreateWipProject(null);
        }

        [Fact]
        public void GetClasses_ShouldReturnListOfClasses()
        {
            // Act
            var result = classService.GetClasses(runnableProject.GetId());

            // Assert
            result.Count.ShouldBe(3);
            result.ShouldContain(c => c.Name == ClassName);
            result.ShouldContain(c => c.Name == BaseClassName);
            result.ShouldContain(c => c.Name == AllFieldsClassName);
        }

        [Fact]
        public void GetClass_ShouldReturnClass_WhenClassExists()
        {
            // Act
            var result = classService.GetClass(runnableProject.GetId(), ClassName);

            // Assert
            result.ShouldNotBeNull();
            result.Name.ShouldBe(ClassName);
        }

        [Fact]
        public void GetClass_ShouldThrowUnknownEntityException_WhenClassDoesNotExist()
        {
            // Act & Assert
            Assert.Throws<UnknownEntityException>(()
                => classService.GetClass(runnableProject.GetId(), "UnknownClass"));
        }

        [Fact]
        public void CreateClassInWipProject_ShouldCreateClass()
        {
            ClassModel expectedClass = new("NewClass", null);
            // Act & Assert
            ClassModel createClassModel = classService.CreateClass(wipProject.GetId(), expectedClass);

            createClassModel.ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedClass);
        }

        [Fact]
        public void UpdateClassInWipProject_ShouldUpdateClass()
        {
            ClassModel expectedClass = new(ClassName, null);
            // Act & Assert
            ClassModel createClassModel = classService.UpdateClass(wipProject.GetId(), expectedClass.Name, expectedClass);

            createClassModel.ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedClass);
        }

        [Fact]
        public void DeleteClassInWipProject_ShouldDeleteClass()
        {
            ClassModel expectedClass = new("NewClass", null);
            // Act
            classService.DeleteClass(wipProject.GetId(), expectedClass.Name);

            // Assert
            wipProject.Classes.ShouldNotContainKey(expectedClass.Name);
        }
    }
}
