using FluentAssertions;
using pva.SuperV.Model;
using pva.SuperV.Model.Exceptions;

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
            instance = null;
            DeleteProject(project);
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
            instance = null;
            DeleteProject(project);
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
            instance = null;
            DeleteProject(project);
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
            wipProject.Version.Should().Be(runnableProject.Version + 1);

            instance.Dispose();
            instance = null;
            DeleteProject(runnableProject);
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
            instance = null;
            DeleteProject(runnableProject);
        }

        [Fact]
        public void GivenWipProjectWithClassInstance_WhenBuildingRunnableProject_ThenInstancesAreRecreated()
        {
            // GIVEN
            RunnableProject runnableProject = CreateRunnableProject();
            Instance instance = runnableProject.CreateInstance(ClassName, InstanceName);
            Field<int> intField = instance.GetField<int>(FieldName);
            intField.Value = 1234;
            WipProject wipProject = Project.CreateProject(runnableProject);

            // WHEN
            runnableProject = ProjectBuilder.Build(wipProject);

            // THEN
            runnableProject.Instances.Should().HaveCount(1);
            instance = runnableProject.GetInstance(InstanceName);
            instance.Should().NotBeNull();
            instance.Class.Name.Should().Be(ClassName);
            intField = instance.GetField<int>(FieldName);
            intField.Value.Should().Be(1234);

            instance.Dispose();
            instance = null;
            DeleteProject(runnableProject);
        }

        [Fact]
        public void GivenProjectWithClassInstance_WhenGettingField_ThenFieldIsReturned()
        {
            // GIVEN
            RunnableProject runnableProject = CreateRunnableProject();
            Instance instance = runnableProject.CreateInstance(ClassName, InstanceName);

            // WHEN
            Field<int> field = instance.GetField<int>(FieldName);

            // THEN
            field.Should().NotBeNull();

            instance.Dispose();
            instance = null;
            DeleteProject(runnableProject);
        }

        [Fact]
        public void GivenProjectWithClassInstance_WhenGettingUnknownField_ThenExceptionIsThrown()
        {
            // GIVEN
            RunnableProject runnableProject = CreateRunnableProject();
            Instance instance = runnableProject.CreateInstance(ClassName, InstanceName);

            // WHEN/THEN
            Assert.Throws<UnknownFieldException>(() => instance.GetField<int>("UnknownField"));

            instance.Dispose();
            instance = null;
            DeleteProject(runnableProject);
        }

        [Fact]
        public void GivenProjectWithClassInstance_WhenGettingFieldWithWrongType_ThenExceptionIsThrown()
        {
            // GIVEN
            RunnableProject runnableProject = CreateRunnableProject();
            Instance instance = runnableProject.CreateInstance(ClassName, InstanceName);

            // WHEN/THEN
            Assert.Throws<WrongFieldTypeException>(() => instance.GetField<double>(FieldName));

            instance.Dispose();
            instance = null;
            DeleteProject(runnableProject);
        }

        private static void DeleteProject(Project project)
        {
            project.Dispose();
#if DELETE_PROJECT
            bool deleted = false;
            for (int i = 0; !deleted && i < 10; i++)
            {
                try
                {
                    File.Delete(project.GetAssemblyFileName());
                    deleted = true;
                }
                catch (Exception ex)
                {
                    Thread.Sleep(i * 100);
                }
            }
#endif
        }

        private static RunnableProject CreateRunnableProject()
        {
            WipProject wipProject = Project.CreateProject(ProjectName);
            Class clazz = wipProject.AddClass(ClassName);
            clazz.AddField(new FieldDefinition<int>(FieldName, 10));
            RunnableProject project = ProjectBuilder.Build(wipProject);
            wipProject.Dispose();
            return project;
        }
    }
}