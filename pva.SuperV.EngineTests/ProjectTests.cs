using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.FieldFormatters;
using pva.SuperV.Engine.FieldValueFormatters;
using Shouldly;

namespace pva.SuperV.EngineTests
{
    public class ProjectTests
    {
        private const string ProjectName = "TestProject";
        private const string ClassName = "TestClass";
        private const string AlarmStatesFormatterName = "AlarmStates";


        [Theory]
        [InlineData("AS.0")]
        [InlineData("0AS")]
        [InlineData("AS-0")]
        public void GivenInvalidProjectName_WhenCreatingProject_ThenInvalidProjectNameExceptionIsThrown(string invalidProjectName)
        {
            // WHEN/THEN
            Assert.Throws<InvalidIdentifierNameException>(() => Project.CreateProject(invalidProjectName));
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

            // WHEN/THEN
            project.AddClass(ClassName);
            project.Classes.ShouldContainKey(ClassName);
            project.Classes[ClassName].ShouldNotBeNull();
            Assert.Throws<EntityAlreadyExistException>(() => project.AddClass(ClassName));
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
            Assert.Throws<UnknownEntityException>(() => project.GetClass(ClassName));
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
        public void GivenProjectWithClassAndFormatter_WhenGettingFormatter_ThenFormatterIsReturned()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            EnumFormatter formatter = new(AlarmStatesFormatterName, ["Closed", "Opened"]);
            project.AddFieldFormatter(formatter);

            // WHEN
            FieldFormatter foundFormatter = project.GetFormatter(AlarmStatesFormatterName);

            // THEN
            foundFormatter.ShouldNotBeNull()
                .ShouldBe(formatter);
        }

        [Fact]
        public void GivenProjectWithClassAndFormatter_WhenGettingUnknownFormatter_ThenExceptionIsThrown()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            EnumFormatter formatter = new(AlarmStatesFormatterName, ["Closed", "Opened"]);
            project.AddFieldFormatter(formatter);

            // WHEN
            Assert.Throws<UnknownEntityException>(() => project.GetFormatter("UnknownFormatter"));
        }

        [Fact]
        public void GivenProjectWithClassAndFormatter_WhenFindingFormatter_ThenFormatterIsReturned()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            EnumFormatter formatter = new(AlarmStatesFormatterName, ["Closed", "Opened"]);
            project.AddFieldFormatter(formatter);

            // WHEN
            FieldFormatter? foundFormatter = project.FindFormatter(AlarmStatesFormatterName);

            // THEN
            foundFormatter.ShouldNotBeNull()
                .ShouldBe(formatter);
        }

        [Fact]
        public void GivenProjectWithClassAndFormatter_WhenFindingUnknownFormatter_ThenNullIsReturned()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            EnumFormatter formatter = new(AlarmStatesFormatterName, ["Closed", "Opened"]);
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
            EnumFormatter formatter = new(AlarmStatesFormatterName, ["Closed", "Opened"]);
            project.AddFieldFormatter(formatter);
            _ = project.AddClass(ClassName);

            // WHEN
            IFieldDefinition field = project.AddField(ClassName, new FieldDefinition<int>("IntField", 10), AlarmStatesFormatterName);

            // THEN
            field.Formatter.ShouldBe(formatter);
        }

        [Fact]
        public void GivenProjectWithClassAndFormatter_WhenAddingFieldWithFormatterWithWrongType_ThenExceptionIsThrown()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            EnumFormatter formatter = new(AlarmStatesFormatterName, ["Closed", "Opened"]);
            project.AddFieldFormatter(formatter);
            _ = project.AddClass(ClassName);

            // WHEN/THEN
            Assert.Throws<InvalidTypeForFormatterException>(() => project.AddField(ClassName, new FieldDefinition<double>("DoubleField", 10.0), AlarmStatesFormatterName));
        }

        [Fact]
        public void GivenProjectWithClassAndFormatter_WhenAddingFieldWithUnknownFormatter_ThenExceptionIsThrown()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            EnumFormatter formatter = new(AlarmStatesFormatterName, ["Closed", "Opened"]);
            project.AddFieldFormatter(formatter);
            _ = project.AddClass(ClassName);

            // WHEN/THEN
            Assert.Throws<UnknownEntityException>(() => project.AddField(ClassName, new FieldDefinition<int>("IntField", 10), "UnknownFormetter"));
        }

        [Fact]
        public void GivenProjectWithClassAndFormatter_WhenAddingFormatterWithSameName_ThenExceptionIsThrown()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            EnumFormatter formatter = new(AlarmStatesFormatterName, ["Closed", "Opened"]);
            project.AddFieldFormatter(formatter);

            // WHEN/THEN
            Assert.Throws<EntityAlreadyExistException>(() => project.AddFieldFormatter(formatter));
        }
    }
}