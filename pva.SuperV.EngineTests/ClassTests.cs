using Shouldly;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;

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
            Assert.Throws<InvalidClassNameException>(() => new Class(invalidClassName));
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
            Assert.Throws<FieldAlreadyExistException>(() => project.AddField(ClassName, new FieldDefinition<int>(FieldName, 10)));
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
            Assert.Throws<UnknownFieldException>(() => clazz.GetField<int>(FieldName));
        }
    }
}