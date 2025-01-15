using Shouldly;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;

namespace pva.SuperV.EngineTests
{
    public class ProjectTests
    {
        private const string ProjectName = "TestProject";
        private const string ClassName = "TestClass";

        [Theory]
        [InlineData("AS.0")]
        [InlineData("0AS")]
        [InlineData("AS-0")]
        public void GivenInvalidProjectName_WhenCreatingProject_ThenInvalidProjectNameExceptionIsThrown(string invalidProjectName)
        {
            // WHEN/THEN
            Assert.Throws<InvalidProjectNameException>(() => Project.CreateProject(invalidProjectName));
        }

        [Fact]
        public void GivenNoCurrentProject_WhenCreatingProject_ThenEmptyProjectIsCreated()
        {
            // WHEN
            WipProject project = Project.CreateProject(ProjectName);

            // THEN
            project.ShouldNotBeNull();
            project.Name.ShouldBe(ProjectName);
            project.Classes.ShouldBeEmpty();
        }

        [Fact]
        public void GivenNewProject_WhenAddingClassToProject_ThenClassIsAdded()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);

            // WHEN
            project.AddClass(ClassName);

            // THEN
            project.Classes.ShouldContainKey(ClassName);
            project.Classes[ClassName].ShouldNotBeNull();
        }

        [Fact]
        public void GivenNewProject_WhenAddingTwiceClassToProject_ThenClassAlreadyExistExceptionIsThrown()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);

            // WHEN
            project.AddClass(ClassName);
            project.Classes.ShouldContainKey(ClassName);
            project.Classes[ClassName].ShouldNotBeNull();
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
            clazz.ShouldNotBeNull();
        }

        [Fact]
        public void GivenNewProject_WhenFindingClassInProject_ThenNullableIsReturned()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);

            // WHEN
            Class? clazz = project.FindClass(ClassName);

            // THEN
            clazz.ShouldBeNull();
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
            clazz.ShouldNotBeNull();
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
            project.Classes.ShouldBeEmpty();
        }

        [Fact]
        public void GivenProjectWithClassAndField_WhenGettingCode_ThenGeneratedCodeIsAsExpected()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            _ = project.AddClass(ClassName);
            project.AddField(ClassName, new FieldDefinition<int>("IntField", 10));

            // WHEN
            string projectCode = project.GetCode();

            // THEN
            string expectedCode = $$"""
using pva.SuperV.Engine;
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

public TestClass() {Fields.Add("IntField", IntField);IntField.Instance = this;
}

}

}

""";
            projectCode.ShouldBe(expectedCode);
        }

        [Fact]
        public void GivenProjectWithClassAndFormatter_WhenGettingFormatter_ThenFormatterIsReturned()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            EnumFormatter formatter = new(ProjectHelpers.AlarmStatesFormatterName, ["Closed", "Opened"]);
            project.AddFieldFormatter(formatter);

            // WHEN
            FieldFormatter foundFormatter = project.GetFormatter(ProjectHelpers.AlarmStatesFormatterName);

            // THEN
            foundFormatter.ShouldNotBeNull()
                .ShouldBe(formatter);
        }

        [Fact]
        public void GivenProjectWithClassAndFormatter_WhenGettingUnknownFormatter_ThenExceptionIsThrown()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            EnumFormatter formatter = new(ProjectHelpers.AlarmStatesFormatterName, ["Closed", "Opened"]);
            project.AddFieldFormatter(formatter);

            // WHEN
            Assert.Throws<UnknownFormatterException>(() => project.GetFormatter("UnknownFormatter"));
        }

        [Fact]
        public void GivenProjectWithClassAndFormatter_WhenFindingFormatter_ThenFormatterIsReturned()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            EnumFormatter formatter = new(ProjectHelpers.AlarmStatesFormatterName, ["Closed", "Opened"]);
            project.AddFieldFormatter(formatter);

            // WHEN
            FieldFormatter? foundFormatter = project.FindFormatter(ProjectHelpers.AlarmStatesFormatterName);

            // THEN
            foundFormatter.ShouldNotBeNull()
                .ShouldBe(formatter);
        }

        [Fact]
        public void GivenProjectWithClassAndFormatter_WhenFindingUnknownFormatter_ThenNullIsReturned()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            EnumFormatter formatter = new(ProjectHelpers.AlarmStatesFormatterName, ["Closed", "Opened"]);
            project.AddFieldFormatter(formatter);

            // WHEN
            FieldFormatter? foundFormatter = project.FindFormatter("UnknownFormatter");

            // THEN
            foundFormatter.ShouldBeNull();
        }

        [Fact]
        public void GivenProjectWithClassAndFormatter_WhenAddingFieldWithFormatter_ThenFieldHasFormatter()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            EnumFormatter formatter = new(ProjectHelpers.AlarmStatesFormatterName, ["Closed", "Opened"]);
            project.AddFieldFormatter(formatter);
            _ = project.AddClass(ClassName);

            // WHEN
            FieldDefinition<int> field = project.AddField(ClassName, new FieldDefinition<int>("IntField", 10), ProjectHelpers.AlarmStatesFormatterName);

            // THEN
            field.Formatter.ShouldBe(formatter);
        }

        [Fact]
        public void GivenProjectWithClassAndFormatter_WhenAddingFieldWithFormatterWithWrongType_ThenExceptionIsThrown()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            EnumFormatter formatter = new(ProjectHelpers.AlarmStatesFormatterName, ["Closed", "Opened"]);
            project.AddFieldFormatter(formatter);
            _ = project.AddClass(ClassName);

            // WHEN/THEN
            Assert.Throws<InvalidTypeForFormatterException>(() => project.AddField(ClassName, new FieldDefinition<double>("DoubleField", 10.0), ProjectHelpers.AlarmStatesFormatterName));
        }

        [Fact]
        public void GivenProjectWithClassAndFormatter_WhenAddingFieldWithUnknownFormatter_ThenExceptionIsThrown()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            EnumFormatter formatter = new(ProjectHelpers.AlarmStatesFormatterName, ["Closed", "Opened"]);
            project.AddFieldFormatter(formatter);
            _ = project.AddClass(ClassName);

            // WHEN/THEN
            Assert.Throws<UnknownFormatterException>(() => project.AddField(ClassName, new FieldDefinition<int>("IntField", 10), "UnknownFormetter"));
        }

        [Fact]
        public void GivenProjectWithClassAndFormatter_WhenAddingFormatterWithSameName_ThenExceptionIsThrown()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            EnumFormatter formatter = new(ProjectHelpers.AlarmStatesFormatterName, ["Closed", "Opened"]);
            project.AddFieldFormatter(formatter);

            // WHEN/THEN
            Assert.Throws<FormatterAlreadyExistException>(() => project.AddFieldFormatter(formatter));
        }
    }
}