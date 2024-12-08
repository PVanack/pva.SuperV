using pva.SuperV.Builder;
using pva.SuperV.Model;

namespace pva.SuperV.BuilderTests
{
    public class ProjectBuilderTests
    {
        private const string ProjectName = "TestProject";
        private const string ClassName = "TestClass";
        private const string InstanceName = "Instance";

        [Fact]
        public void GivenProjectWithClassAndField_WhenBuildingAndCreatingClassInstance_ThenInstanceIsCreated()
        {
            // GIVEN
            WipProject wipProject = Project.CreateProject(ProjectName);
            Class clazz = wipProject.AddClass(ClassName);
            clazz.AddField(new FieldDefinition<int>("IntField", 10));

            // WHEN
            RunnableProject project = ProjectBuilder.Build(wipProject);
            var instance = project.CreateClassInstance(ClassName, InstanceName);

            // THEN
            Assert.NotNull(instance);
            Assert.Equal(InstanceName, instance.Name);
            Assert.Equal(10, instance.IntField.Value);
        }
    }
}