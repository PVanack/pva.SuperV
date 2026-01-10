using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.Processing;
using Shouldly;

namespace pva.SuperV.EngineTests
{
    public class ClassTests
    {
        private const string ProjectName = "TestProject";
        private const string ClassName = "TestClass";
        private const string FieldName = "IntField";

        [Theory]
        [InlineData("AS.0")]
        [InlineData("0AS")]
        [InlineData("AS-0")]
        public void GivenInvalidClassName_WhenCreatingClass_ThenInvalidClassNameExceptionIsThrown(string invalidClassName)
        {
            // WHEN/THEN
            Should.Throw<InvalidIdentifierNameException>(() => new Class(invalidClassName));
        }

        [Fact]
        public void GivenEmptyProject_WhenCreatingClass_ThenClassIsCreatedWithNoField()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);

            // WHEN
            Class clazz = project.AddClass(ClassName);

            // THEN
            clazz.ShouldNotBeNull()
                .Name.ShouldBe(ClassName);
            clazz.FieldDefinitions.ShouldNotBeNull();
            clazz.FieldDefinitions.ShouldBeEmpty();
        }

        [Fact]
        public void GivenClassWithNoField_WhenAddingField_ThenFieldIsAdded()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            Class clazz = project.AddClass(ClassName);

            // WHEN
            project.AddField(ClassName, new FieldDefinition<int>(FieldName, 10));

            // THEN
            clazz.FieldDefinitions.ShouldNotBeNull()
                .ShouldContainKey(FieldName);
            FieldDefinition<int>? field = clazz.GetField<int>(FieldName);
            field!.ShouldNotBeNull();
            field!.DefaultValue.ShouldBe(10);
        }

        [Fact]
        public void GivenClassWithField_WhenAddingSameField_ThenExceptionIsThrown()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            _ = project.AddClass(ClassName);
            project.AddField(ClassName, new FieldDefinition<int>(FieldName, 10));

            // WHEN/THEN
            Should.Throw<EntityAlreadyExistException>(() => project.AddField(ClassName, new FieldDefinition<int>(FieldName, 10)));
        }

        [Fact]
        public void GivenClassWithField_WhenUpdatingField_ThenFieldIsUpdated()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            Class clazz = project.AddClass(ClassName);
            project.AddField(ClassName, new FieldDefinition<int>(FieldName, 10));

            // WHEN
            project.UpdateField(ClassName, FieldName, new FieldDefinition<int>(FieldName, 20), null);

            // THEN
            clazz.FieldDefinitions.ShouldNotBeNull()
                .ShouldContainKey(FieldName);
            FieldDefinition<int>? field = clazz.GetField<int>(FieldName);
            field!.ShouldNotBeNull();
            field!.DefaultValue.ShouldBe(20);
            field.Formatter.ShouldBeNull();
        }

        [Fact]
        public void GivenClassWithField_WhenUpdatingFieldWithAnotherType_ThenExceptionIsThrown()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            _ = project.AddClass(ClassName);
            project.AddField(ClassName, new FieldDefinition<int>(FieldName, 10));

            // WHEN/THEN
            Should.Throw<WrongFieldTypeException>(() => project.UpdateField(ClassName, FieldName, new FieldDefinition<float>(FieldName, 10), null));
        }

        [Fact]
        public void GivenClassWithField_WhenRemovingField_ThenFieldIsRemoved()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            Class clazz = project.AddClass(ClassName);
            project.AddField(ClassName, new FieldDefinition<int>(FieldName, 10));

            // WHEN
            project.RemoveField(ClassName, FieldName);

            // THEN
            clazz.FieldDefinitions.ShouldBeEmpty();
            Should.Throw<UnknownEntityException>(() => clazz.GetField<int>(FieldName));
        }

        [Fact]
        public void GivenClassWithFieldUsedInProcessing_WhenRemovingField_ThenExceptionIsThrown()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            project.AddHistoryRepository(new Engine.HistoryStorage.HistoryRepository("HistoryRepository"));
            Class clazz = project.AddClass(ClassName);
            project.AddField(ClassName, new FieldDefinition<int>("ValueField", 10));
            project.AddField(ClassName, new FieldDefinition<int>(FieldName, 10));
            project.AddFieldChangePostProcessing(ClassName, "ValueField", new HistorizationProcessing<int>("Historization", project, clazz, "ValueField", "HistoryRepository",
                null, ["ValueField", FieldName]));

            // WHEN/THEN
            Should.Throw<EntityInUseException>(() => project.RemoveField(ClassName, FieldName));
        }
    }
}