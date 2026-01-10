using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using Shouldly;

namespace pva.SuperV.EngineTests
{
    [Collection("Project building")]
    public class ProjectBuilderTests : SuperVTestsBase
    {
        [Fact]
        public void GivenProjectWithClassAndField_WhenBuildingAndCreatingClassInstance_ThenInstanceIsCreatedAndHasInheritedProperties()
        {
            // GIVEN
            RunnableProject project = CreateRunnableProject();

            // WHEN
            var instance = project.CreateInstance(ClassName, InstanceName) as dynamic;
            var retrievedInstance = project.GetInstance(InstanceName);

            // THEN
            Assert.NotNull(instance);
            Assert.Equal(InstanceName, instance!.Name!);
            Assert.Equal(10, instance.Value.Value);
            Assert.Equal(1, instance.AlarmState.Value);
            Assert.Equal("High", instance.AlarmState.ToString());
            Assert.Equal(instance, retrievedInstance);

            Assert.Equal("InheritedField", instance.InheritedField.Value);

            instance.Dispose();
            DeleteProject(project);
        }

        [Fact]
        public void GivenProjectWithClassInstance_WhenRemovingClassInstance_ThenInstanceIsRemoved()
        {
            // GIVEN
            RunnableProject project = CreateRunnableProject();
            var instance = project.CreateInstance(ClassName, InstanceName);

            // WHEN
            project.RemoveInstance(instance!.Name!);

            // THEN
            project.Instances.ShouldBeEmpty();

            instance?.Dispose();
            DeleteProject(project);
        }

        [Fact]
        public void GivenProjectWithClassInstance_WhenGettingWrongInstance_ThenExceptionIsThrown()
        {
            // GIVEN
            RunnableProject project = CreateRunnableProject();
            var instance = project.CreateInstance(ClassName, InstanceName);

            // WHEN/THEN
            Assert.Throws<UnknownEntityException>(() => project.GetInstance("WrongInstance"));

            instance?.Dispose();
            DeleteProject(project);
        }

        [Fact]
        public void GivenProjectWithClassInstance_WheCreatingInstanceWithSameName_ThenExceptionIsThrown()
        {
            // GIVEN
            RunnableProject project = CreateRunnableProject();
            var instance = project.CreateInstance(ClassName, InstanceName);

            // WHEN/THEN
            Assert.Throws<EntityAlreadyExistException>(() => project.CreateInstance(ClassName, InstanceName));

            instance?.Dispose();
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
            wipProject.Name.ShouldBe(runnableProject.Name);
            wipProject.Description.ShouldBe(runnableProject.Description);
            wipProject.Version.ShouldBe(runnableProject.Version + 1);

            instance?.Dispose();
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
            wipProject.ToLoadInstances.ShouldBeEmpty();

            instance?.Dispose();
            DeleteProject(runnableProject);
        }

        [Fact]
        public async Task GivenWipProjectWithClassInstance_WhenBuildingRunnableProject_ThenInstancesAreRecreated()
        {
            // GIVEN
            RunnableProject runnableProject = CreateRunnableProject();
            Instance? instance = runnableProject.CreateInstance(ClassName, InstanceName);
            Field<int>? intField = instance?.GetField<int>(ValueFieldName);
            intField!.SetValue(1234);
            WipProject wipProject = Project.CreateProject(runnableProject);

            // WHEN
            runnableProject = await Project.BuildAsync(wipProject);

            // THEN
            runnableProject.Instances.Count.ShouldBe(1);
            instance = runnableProject.GetInstance(InstanceName);
            instance.ShouldNotBeNull();
            instance.Class.Name.ShouldBe(ClassName);
            intField = instance.GetField<int>(ValueFieldName);
            intField!.Value.ShouldBe(1234);

            instance?.Dispose();
            DeleteProject(runnableProject);
        }

        [Fact]
        public async Task GivenWipProject_WhenBuildingRunnableProject_ThenFieldDefinitionsWithTopicHaveNotificationChannelCreated()
        {
            // GIVEN
            RunnableProject runnableProject = CreateRunnableProject();
            Instance? instance = runnableProject.CreateInstance(ClassName, InstanceName);
            Field<int>? intField = instance?.GetField<int>(ValueFieldName);
            intField!.SetValue(1234);
            WipProject wipProject = Project.CreateProject(runnableProject);

            // WHEN
            runnableProject = await Project.BuildAsync(wipProject);

            // THEN
            IFieldDefinition fieldWithTopic = runnableProject.GetClass(ClassName).GetField(ValueFieldName);
            fieldWithTopic.TopicName.ShouldBe("TopicName");
            fieldWithTopic.FieldValueChangedEventChannel.ShouldNotBeNull();

            instance?.Dispose();
            DeleteProject(runnableProject);
        }

        [Fact]
        public void GivenProjectWithClassInstance_WhenGettingField_ThenFieldIsReturned()
        {
            // GIVEN
            RunnableProject runnableProject = CreateRunnableProject();
            Instance? instance = runnableProject.CreateInstance(ClassName, InstanceName);

            // WHEN
            Field<int>? field = instance?.GetField<int>(ValueFieldName);

            // THEN
            field.ShouldNotBeNull();

            instance?.Dispose();
            DeleteProject(runnableProject);
        }

        [Fact]
        public void GivenProjectWithClassInstance_WhenGettingUnknownField_ThenExceptionIsThrown()
        {
            // GIVEN
            RunnableProject runnableProject = CreateRunnableProject();
            Instance? instance = runnableProject.CreateInstance(ClassName, InstanceName);

            // WHEN/THEN
            Assert.Throws<UnknownEntityException>(() => instance!.GetField<int>("UnknownField"));

            instance?.Dispose();
            DeleteProject(runnableProject);
        }

        [Fact]
        public void GivenProjectWithClassInstance_WhenGettingFieldWithWrongType_ThenExceptionIsThrown()
        {
            // GIVEN
            RunnableProject runnableProject = CreateRunnableProject();
            Instance? instance = runnableProject.CreateInstance(ClassName, InstanceName);

            // WHEN/THEN
            Assert.Throws<WrongFieldTypeException>(() => instance!.GetField<double>(ValueFieldName));

            instance?.Dispose();
            DeleteProject(runnableProject);
        }

        [Fact]
        public void GivenProjectWithClassInstance_WhenSettingFieldValue_ThenValueChangeProcessingIsPerformed()
        {
            // GIVEN
            RunnableProject runnableProject = CreateRunnableProject();
            dynamic? instance = runnableProject.CreateInstance(ClassName, InstanceName);

            // WHEN
            instance!.Value.SetValue(50);
            instance!.Value.SetValue(110);

            // THEN
            int alarmState = instance.AlarmState.Value;
            alarmState.ShouldBe(2);

            instance?.Dispose();
            DeleteProject(runnableProject);
        }
    }
}