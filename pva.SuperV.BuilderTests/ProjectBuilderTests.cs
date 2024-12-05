using pva.SuperV.Builder;
using pva.SuperV.Model;

namespace pva.SuperV.BuilderTests
{
    public class ProjectBuilderTests
    {
        private const string PROJECT_NAME = "TestProject";
        private const string CLASS_NAME = "TestClass";
        private const string INSTANCE_NAME = "Instance";

        [Fact]
        public void GivenProjectWithClassAndField_WhenBuildingAndCreatingClassInstance_ThenInstanceIsCreated()
        {
            // GIVEN
            WipProject wipProject = Project.CreateProject(PROJECT_NAME);
            Class clazz = wipProject.AddClass(CLASS_NAME);
            clazz.AddField(new FieldDefinition<int>("IntField", 10));

            // WHEN
            RunnableProject project = ProjectBuilder.Build(wipProject);
            var instance = project.CreateClassInstance(CLASS_NAME, INSTANCE_NAME);

            // THEN
            Assert.NotNull(instance);
            Assert.Equal(INSTANCE_NAME, instance.Name);
            Assert.Equal(10, instance.IntField.Value);
        }
    }
}