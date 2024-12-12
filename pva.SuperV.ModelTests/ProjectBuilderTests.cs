using FluentAssertions;
using pva.SuperV.Model;
using pva.SuperV.Model.Exceptions;
using System.Runtime.Loader;
using System.Security.Principal;

namespace pva.SuperV.ModelTests
{
    public class ProjectBuilderTests
    {
        private const string ProjectName = "TestProject";
        private const string ClassName = "TestClass";
        private const string InstanceName = "Instance";
        private const string FieldName = "IntField";

        [Fact]
        public void GivenProjectWithClassAndField_WhenBuildingAndCreatingClassInstance_ThenInstanceIsCreated()
        {
            // GIVEN

            // WHEN
            RunnableProject project = CreateRunnableProject();
            var instance = project.CreateInstance(ClassName, InstanceName);
            var retrievedInstance = project.GetInstance(InstanceName);

            // THEN
            Assert.NotNull(instance);
            Assert.Equal(InstanceName, instance.Name);
            Assert.Equal(10, instance.IntField.Value);
            Assert.Equal(instance, retrievedInstance);

            instance.Dispose();
            project.Dispose();
        }

        [Fact]
        public void GivenProjectWithClassInstance_WhenRemovingClassInstance_ThenInstanceIsRemoved()
        {
            // GIVEN
            RunnableProject project = CreateRunnableProject();
            var instance = project.CreateInstance(ClassName, InstanceName);

            // WHEN
            project.RemoveInstance(instance.Name);

            // THEN
            project.Instances.Should().BeEmpty();

            instance.Dispose();
            project.Dispose();
        }

        [Fact]
        public void GivenProjectWithClassInstance_WhenGettingWrongInstance_ThenExceptionIsThrown()
        {
            // GIVEN
            RunnableProject project = CreateRunnableProject();
            var instance = project.CreateInstance(ClassName, InstanceName);

            // WHEN/THEN
            Assert.Throws<UnknownInstanceException>(() => project.GetInstance("WrongInstance"));


            instance.Dispose();
            project.Dispose();
        }

        [Fact]
        public void GivenRunnableProjectWithClassInstance_WhenCreatingWipFromIt_ThenWipProjectIsCorrectlySetup()
        {
            // GIVEN
            RunnableProject runnableProject = CreateRunnableProject();
            var instance = runnableProject.CreateInstance(ClassName, InstanceName);

            // WHEN
            WipProject wipProject = Project.CreateProject(runnableProject);

            // THEN
            wipProject.Name.Should().Be(runnableProject.Name);
            wipProject.Description.Should().Be(runnableProject.Description);
            wipProject.Version.Should().Be(runnableProject.Version+1);

            instance.Dispose();
            runnableProject.Dispose();
        }

        [Fact]
        public void GivenWipProjectWithClassInstance_WhenRemovingClassFromIt_ThenToLoadInstancesIsEmpty()
        {
            // GIVEN
            RunnableProject runnableProject = CreateRunnableProject();
            var instance = runnableProject.CreateInstance(ClassName, InstanceName);
            WipProject wipProject = Project.CreateProject(runnableProject);

            // WHEN
            wipProject.RemoveClass(ClassName);

            // THEN
            wipProject.ToLoadInstances.Should().BeEmpty();

            instance.Dispose();
            runnableProject.Dispose();
        }

        [Fact]
        public void GivenWipProjectWithClassInstance_WhenGettingField_ThenFieldIsReturned()
        {
            // GIVEN
            RunnableProject runnableProject = CreateRunnableProject();
            Instance instance = runnableProject.CreateInstance(ClassName, InstanceName);

            // WHEN
            Field<int> field = instance.GetField<int>(FieldName);

            // THEN
            field.Should().NotBeNull();

            instance.Dispose();
            runnableProject.Dispose();
        }

        [Fact]
        public void GivenWipProjectWithClassInstance_WhenGettingUnknownField_ThenExceptionIsThrown()
        {
            // GIVEN
            RunnableProject runnableProject = CreateRunnableProject();
            Instance instance = runnableProject.CreateInstance(ClassName, InstanceName);

            // WHEN/THEN
            Assert.Throws<UnknownFieldException>(() => instance.GetField<int>("UnknownField"));

            instance.Dispose();
            runnableProject.Dispose();
        }

        [Fact]
        public void GivenWipProjectWithClassInstance_WhenGettingFieldWithWrongType_ThenExceptionIsThrown()
        {
            // GIVEN
            RunnableProject runnableProject = CreateRunnableProject();
            Instance instance = runnableProject.CreateInstance(ClassName, InstanceName);

            // WHEN/THEN
            Assert.Throws<WrongFieldTypeException>(() => instance.GetField<double>(FieldName));

            instance.Dispose();
            runnableProject.Dispose();
        }

        private static RunnableProject CreateRunnableProject()
        {
            WipProject wipProject = Project.CreateProject(ProjectName);
            Class clazz = wipProject.AddClass(ClassName);
            clazz.AddField(new FieldDefinition<int>(FieldName, 10));
            RunnableProject project = ProjectBuilder.Build(wipProject);
            return project;
        }
    }
}