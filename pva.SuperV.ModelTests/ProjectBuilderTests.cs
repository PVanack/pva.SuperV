using FluentAssertions;
using pva.SuperV.Model;
using System.Runtime.Loader;

namespace pva.SuperV.ModelTests
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
            var instance = project.CreateInstance(ClassName, InstanceName);

            // THEN
            Assert.NotNull(instance);
            Assert.Equal(InstanceName, instance.Name);
            Assert.Equal(10, instance.IntField.Value);

            instance.Dispose();
            project.Unload();
        }

        [Fact]
        public void GivenProjectWithClassInstance_WhenRemovingClassInstance_ThenInstanceIsRemoved()
        {
            // GIVEN
            WipProject wipProject = Project.CreateProject(ProjectName);
            var z = AssemblyLoadContext.Default.Assemblies;
            Class clazz = wipProject.AddClass(ClassName);
            clazz.AddField(new FieldDefinition<int>("IntField", 10));
            RunnableProject project = ProjectBuilder.Build(wipProject);
            var instance = project.CreateInstance(ClassName, InstanceName);

            // WHEN
            project.RemoveInstance(instance.Name);

            // THEN
            project.Instances.Should().BeEmpty();

            instance.Dispose();
            project.Unload();
        }
    }
}