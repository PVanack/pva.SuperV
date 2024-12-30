using FluentAssertions;
using pva.SuperV.Model;
using pva.SuperV.Model.Exceptions;

namespace pva.SuperV.ModelTests
{
    public class ProjectTests
    {
        private const string ProjectName = "TestProject";
        private const string ClassName = "TestClass";

        [Fact]
        public void GivenInvalidProjectName_WhenCreatingProject_ThenInvalidProjectNameExceptionIsThrown()
        {
            // WHEN/THEN
            Assert.Throws<InvalidProjectNameException>(() => Project.CreateProject("AS.@"));
        }

        [Fact]
        public void GivenNoCurrentProject_WhenCreatingProject_ThenEmptyProjectIsCreated()
        {
            // WHEN
            WipProject project = Project.CreateProject(ProjectName);

            // THEN
            project.Should().NotBeNull();
            project.Name.Should().Be(ProjectName);
            project.Classes.Should().BeEmpty();
        }

        [Fact]
        public void GivenNewProject_WhenAddingClassToProject_ThenClassIsAdded()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);

            // WHEN
            project.AddClass(ClassName);

            // THEN
            project.Classes.Should().Satisfy(entry => entry.Key.Equals(ClassName));
            project.Classes[ClassName].Should().NotBeNull();
        }

        [Fact]
        public void GivenNewProject_WhenAddingTwiceClassToProject_ThenClassAlreadyExistExceptionIsThrown()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);

            // WHEN
            project.AddClass(ClassName);
            project.Classes.Should().Satisfy(entry => entry.Key.Equals(ClassName));
            project.Classes[ClassName].Should().NotBeNull();
            var exception = Assert.Throws<ClassAlreadyExistException>(() => project.AddClass(ClassName));

            // THEN
            Assert.IsType<ClassAlreadyExistException>(exception);
        }

        [Fact]
        public void GivenNewProjectWithClass_WhenFindingClassInProject_ThenClassIsReturned()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            project.AddClass(ClassName);

            // WHEN
            Class? clazz = project.FindClass(ClassName);

            // THEN
            clazz.Should().NotBeNull();
        }

        [Fact]
        public void GivenNewProject_WhenFindingClassInProject_ThenNullableIsReturned()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);

            // WHEN
            Class? clazz = project.FindClass(ClassName);

            // THEN
            clazz.Should().BeNull();
        }

        [Fact]
        public void GivenNewProjectWithClass_WhenGettingClassInProject_ThenClassIsReturned()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            project.AddClass(ClassName);

            // WHEN
            Class clazz = project.GetClass(ClassName);

            // THEN
            clazz.Should().NotBeNull();
        }

        [Fact]
        public void GivenNewProject_WhenGettingClassInProject_ThenUnknownClassExceptionIsThrown()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);

            // WHEN
            var exception = Assert.Throws<UnknownClassException>(() => project.GetClass(ClassName));

            // THEN
            Assert.IsType<UnknownClassException>(exception);
        }

        [Fact]
        public void GivenProjectWithClass_WhenRemovingClassInProject_ThenClassIsRemoved()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            project.AddClass(ClassName);

            // WHEN
            project.RemoveClass(ClassName);

            // THEN
            project.Classes.Should().BeEmpty();
        }

        [Fact]
        public void GivenProjectWithClassAndField_WhenGettingCode_ThenGeneratedCodeIsAsExpected()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            Class clazz = project.AddClass(ClassName);
            clazz.AddField(new FieldDefinition<int>("IntField", 10));

            // WHEN
            string projectCode = project.GetCode();

            // THEN
            string expectedCode = $$"""
using pva.SuperV.Model;
using System.Collections.Generic;
using System.Reflection;
[assembly: AssemblyProduct("pva.SuperV")]
[assembly: AssemblyTitle("{{project.Description}}")]
[assembly: AssemblyVersion("{{project.Version}}")]
[assembly: AssemblyFileVersion("{{project.Version}}")]
[assembly: AssemblyInformationalVersion("{{project.Version}}")]
namespace {{ProjectName}}.V{{project.Version}} {
public class {{ClassName}} : Instance {
public Field<System.Int32> IntField { get; set; } = new(10);

public TestClass() {Fields.Add("IntField", IntField);
}

}

}

""";
            projectCode.Should().BeEquivalentTo(expectedCode);
        }
    }
}