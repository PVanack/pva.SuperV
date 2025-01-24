using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using Shouldly;

namespace pva.SuperV.EngineTests
{
    [Collection("Project building")]
    public class ProjectBuilderTests
    {
        [Fact]
        public void GivenProjectWithClassAndField_WhenBuildingAndCreatingClassInstance_ThenInstanceIsCreatedAndHasInheritedProperties()
        {
            // GIVEN
            RunnableProject project = ProjectHelpers.CreateRunnableProject();

            // WHEN
            var instance = project.CreateInstance(ProjectHelpers.ClassName, ProjectHelpers.InstanceName);
            var retrievedInstance = project.GetInstance(ProjectHelpers.InstanceName);

            // THEN
            Assert.NotNull(instance);
            Assert.Equal(ProjectHelpers.InstanceName, instance!.Name!);
            Assert.Equal(10, instance.Value.Value);
            Assert.Equal(1, instance.AlarmState.Value);
            Assert.Equal("High", instance.AlarmState.ToString());
            Assert.Equal(instance, retrievedInstance);

            Assert.Equal("InheritedField", instance.InheritedField.Value);

            instance.Dispose();
            ProjectHelpers.DeleteProject(project);
        }

        [Fact]
        public void GivenProjectWithClassInstance_WhenRemovingClassInstance_ThenInstanceIsRemoved()
        {
            // GIVEN
            RunnableProject project = ProjectHelpers.CreateRunnableProject();
            var instance = project.CreateInstance(ProjectHelpers.ClassName, ProjectHelpers.InstanceName);

            // WHEN
            project.RemoveInstance(instance!.Name!);

            // THEN
            project.Instances.ShouldBeEmpty();

            instance?.Dispose();
            ProjectHelpers.DeleteProject(project);
        }

        [Fact]
        public void GivenProjectWithClassInstance_WhenGettingWrongInstance_ThenExceptionIsThrown()
        {
            // GIVEN
            RunnableProject project = ProjectHelpers.CreateRunnableProject();
            var instance = project.CreateInstance(ProjectHelpers.ClassName, ProjectHelpers.InstanceName);

            // WHEN/THEN
            Assert.Throws<UnknownEntityException>(() => project.GetInstance("WrongInstance"));

            instance?.Dispose();
            ProjectHelpers.DeleteProject(project);
        }

        [Fact]
        public void GivenProjectWithClassInstance_WheCreatingInstanceWithSameName_ThenExceptionIsThrown()
        {
            // GIVEN
            RunnableProject project = ProjectHelpers.CreateRunnableProject();
            var instance = project.CreateInstance(ProjectHelpers.ClassName, ProjectHelpers.InstanceName);

            // WHEN/THEN
            Assert.Throws<EntityAlreadyExistException>(() => project.CreateInstance(ProjectHelpers.ClassName, ProjectHelpers.InstanceName));

            instance?.Dispose();
            ProjectHelpers.DeleteProject(project);
        }

        [Fact]
        public void GivenRunnableProjectWithClassInstance_WhenCreatingWipFromIt_ThenWipProjectIsCorrectlySetup()
        {
            // GIVEN
            RunnableProject runnableProject = ProjectHelpers.CreateRunnableProject();
            var instance = runnableProject.CreateInstance(ProjectHelpers.ClassName, ProjectHelpers.InstanceName);

            // WHEN
            WipProject wipProject = Project.CreateProject(runnableProject);

            // THEN
            wipProject.Name.ShouldBe(runnableProject.Name);
            wipProject.Description.ShouldBe(runnableProject.Description);
            wipProject.Version.ShouldBe(runnableProject.Version + 1);

            instance?.Dispose();
            ProjectHelpers.DeleteProject(runnableProject);
        }

        [Fact]
        public void GivenWipProjectWithClassInstance_WhenRemovingClassFromIt_ThenToLoadInstancesIsEmpty()
        {
            // GIVEN
            RunnableProject runnableProject = ProjectHelpers.CreateRunnableProject();
            var instance = runnableProject.CreateInstance(ProjectHelpers.ClassName, ProjectHelpers.InstanceName);
            WipProject wipProject = Project.CreateProject(runnableProject);

            // WHEN
            wipProject.RemoveClass(ProjectHelpers.ClassName);

            // THEN
            wipProject.ToLoadInstances.ShouldBeEmpty();

            instance?.Dispose();
            ProjectHelpers.DeleteProject(runnableProject);
        }

        [Fact]
        public void GivenWipProjectWithClassInstance_WhenBuildingRunnableProject_ThenInstancesAreRecreated()
        {
            // GIVEN
            RunnableProject runnableProject = ProjectHelpers.CreateRunnableProject();
            Instance? instance = runnableProject.CreateInstance(ProjectHelpers.ClassName, ProjectHelpers.InstanceName);
            Field<int>? intField = instance?.GetField<int>(ProjectHelpers.ValueFieldName);
            intField!.SetValue(1234);
            WipProject wipProject = Project.CreateProject(runnableProject);

            // WHEN
            runnableProject = ProjectBuilder.Build(wipProject);

            // THEN
            runnableProject.Instances.Count.ShouldBe(1);
            instance = runnableProject.GetInstance(ProjectHelpers.InstanceName);
            instance.ShouldNotBeNull();
            instance.Class.Name.ShouldBe(ProjectHelpers.ClassName);
            intField = instance.GetField<int>(ProjectHelpers.ValueFieldName);
            intField!.Value.ShouldBe(1234);

            instance?.Dispose();
            ProjectHelpers.DeleteProject(runnableProject);
        }

        [Fact]
        public void GivenProjectWithClassInstance_WhenGettingField_ThenFieldIsReturned()
        {
            // GIVEN
            RunnableProject runnableProject = ProjectHelpers.CreateRunnableProject();
            Instance? instance = runnableProject.CreateInstance(ProjectHelpers.ClassName, ProjectHelpers.InstanceName);

            // WHEN
            Field<int>? field = instance?.GetField<int>(ProjectHelpers.ValueFieldName);

            // THEN
            field.ShouldNotBeNull();

            instance?.Dispose();
            ProjectHelpers.DeleteProject(runnableProject);
        }

        [Fact]
        public void GivenProjectWithClassInstance_WhenGettingUnknownField_ThenExceptionIsThrown()
        {
            // GIVEN
            RunnableProject runnableProject = ProjectHelpers.CreateRunnableProject();
            Instance? instance = runnableProject.CreateInstance(ProjectHelpers.ClassName, ProjectHelpers.InstanceName);

            // WHEN/THEN
            Assert.Throws<UnknownEntityException>(() => instance!.GetField<int>("UnknownField"));

            instance?.Dispose();
            ProjectHelpers.DeleteProject(runnableProject);
        }

        [Fact]
        public void GivenProjectWithClassInstance_WhenGettingFieldWithWrongType_ThenExceptionIsThrown()
        {
            // GIVEN
            RunnableProject runnableProject = ProjectHelpers.CreateRunnableProject();
            Instance? instance = runnableProject.CreateInstance(ProjectHelpers.ClassName, ProjectHelpers.InstanceName);

            // WHEN/THEN
            Assert.Throws<WrongFieldTypeException>(() => instance!.GetField<double>(ProjectHelpers.ValueFieldName));

            instance?.Dispose();
            ProjectHelpers.DeleteProject(runnableProject);
        }

        [Fact]
        public void GivenProjectWithClassInstance_WhenSettingFieldValue_ThenValueChangeProcessingIsPerformed()
        {
            // GIVEN
            RunnableProject runnableProject = ProjectHelpers.CreateRunnableProject();
            dynamic? instance = runnableProject.CreateInstance(ProjectHelpers.ClassName, ProjectHelpers.InstanceName);

            // WHEN
            instance!.Value.SetValue(50);
            instance!.Value.SetValue(110);

            // THEN
            int alarmState = instance.AlarmState.Value;
            alarmState.ShouldBe(2);

            instance?.Dispose();
            ProjectHelpers.DeleteProject(runnableProject);
        }
    }
}