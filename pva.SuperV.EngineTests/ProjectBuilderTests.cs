using FluentAssertions;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;

namespace pva.SuperV.EngineTests
{
    [Collection("Project building")]
    public class ProjectBuilderTests
    {
        [Fact]
        public void GivenProjectWithClassAndField_WhenBuildingAndCreatingClassInstance_ThenInstanceIsCreated()
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
            project.Instances.Should().BeEmpty();

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
            Assert.Throws<UnknownInstanceException>(() => project.GetInstance("WrongInstance"));

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
            Assert.Throws<InstanceAlreadyExistException>(() => project.CreateInstance(ProjectHelpers.ClassName, ProjectHelpers.InstanceName));

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
            wipProject.Name.Should().Be(runnableProject.Name);
            wipProject.Description.Should().Be(runnableProject.Description);
            wipProject.Version.Should().Be(runnableProject.Version + 1);

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
            wipProject.ToLoadInstances.Should().BeEmpty();

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
            runnableProject.Instances.Should().HaveCount(1);
            instance = runnableProject.GetInstance(ProjectHelpers.InstanceName);
            instance.Should().NotBeNull();
            instance.Class.Name.Should().Be(ProjectHelpers.ClassName);
            intField = instance.GetField<int>(ProjectHelpers.ValueFieldName);
            intField!.Value.Should().Be(1234);

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
            field.Should().NotBeNull();

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
            Assert.Throws<UnknownFieldException>(() => instance!.GetField<int>("UnknownField"));

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
            alarmState.Should().Be(2);

            instance?.Dispose();
            ProjectHelpers.DeleteProject(runnableProject);
        }
    }
}