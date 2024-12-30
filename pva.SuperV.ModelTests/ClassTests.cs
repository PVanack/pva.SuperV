using FluentAssertions;
using pva.SuperV.Model;
using pva.SuperV.Model.Exceptions;

namespace pva.SuperV.ModelTests
{
    public class ClassTests
    {
        private const string ProjectName = "TestProject";
        private const string ClassName = "TestClass";
        private const string FieldName = "IntField";

        [Fact]
        public void GivenInvalidClassName_WhenCreatingClass_ThenInvalidClassNameExceptionIsThrown()
        {
            // WHEN/THEN
            Assert.Throws<InvalidClassNameException>(() => new Class("AZ.0"));
        }

        [Fact]
        public void GivenEmptyProject_WhenCreatingClass_ThenClassIsCreatedWithNoField()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);

            // WHEN
            Class clazz = project.AddClass(ClassName);

            // THEN
            clazz.Should().NotBeNull();
            clazz.Name.Should().Be(ClassName);
            clazz.FieldDefinitions.Should().NotBeNull();
            clazz.FieldDefinitions.Should().BeEmpty();
        }

        [Fact]
        public void GivenClassWithNoField_WhenAddingField_ThenFieldIsAdded()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            Class clazz = project.AddClass(ClassName);

            // WHEN
            clazz.AddField(new FieldDefinition<int>(FieldName, 10));

            // THEN
            clazz.FieldDefinitions.Should().NotBeNull();
            clazz.FieldDefinitions.Should().ContainKey(FieldName);
            FieldDefinition<int>? field = clazz.GetField<int>(FieldName);
            field!.Should().NotBeNull();
            field!.DefaultValue.Should().Be(10);
        }

        [Fact]
        public void GivenClassWithField_WhenAddingSameField_ThenExceptionIsThrown()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            Class clazz = project.AddClass(ClassName);
            clazz.AddField(new FieldDefinition<int>(FieldName, 10));

            // WHEN/THEN
            Assert.Throws<FieldAlreadyExistException>(() => clazz.AddField(new FieldDefinition<int>(FieldName, 10)));
        }

        [Fact]
        public void GivenClassWithField_WhenRemovingField_ThenFieldIsRemoved()
        {
            // GIVEN
            WipProject project = Project.CreateProject(ProjectName);
            Class clazz = project.AddClass(ClassName);
            clazz.AddField(new FieldDefinition<int>(FieldName, 10));

            // WHEN
            clazz.RemoveField(FieldName);

            // THEN
            clazz.FieldDefinitions.Should().BeEmpty();
            Assert.Throws<UnknownFieldException>(() => clazz.GetField<int>(FieldName));
        }
    }
}