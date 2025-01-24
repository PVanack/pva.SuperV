using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using Shouldly;

namespace pva.SuperV.EngineTests
{
    [Collection("Project building")]

    public class FieldTests
    {
        [Theory]
        [InlineData("AS.0")]
        [InlineData("0AS")]
        [InlineData("AS-0")]
        public void GivenInvalidFieldName_WhenCreatingField_ThenInvalidFieldNameExceptionIsThrown(string invalidFieldName)
        {
            // WHEN/THEN
            Assert.Throws<InvalidIdentifierNameException>(() => new FieldDefinition<int>(invalidFieldName, 10));
        }

        [Fact]
        public void BoolField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<bool>(true);
        }

        [Fact]
        public void ShortField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<short>(-1234);
        }

        [Fact]
        public void UshortField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<ushort>(1234);
        }

        [Fact]
        public void IntField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<int>(-1234);
        }

        [Fact]
        public void UintField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<uint>(1234);
        }

        [Fact]
        public void LongField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<long>(-1234);
        }

        [Fact]
        public void UlongField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<ulong>(1234);
        }

        [Fact]
        public void FloatField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<float>(1234.4321f);
        }

        [Fact]
        public void DoubleField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<double>(1234.4321);
        }

        [Fact]
        public void StringField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<string>("Some text");
        }

        [Fact]
        public void DateTimeField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<DateTime>(DateTime.Now);
        }

        [Fact]
        public void TimeSpanField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<TimeSpan>(TimeSpan.FromMinutes(1.0));
        }

        private static void GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<T>(T fieldValue)
        {
            WipProject wipProject = Project.CreateProject(ProjectHelpers.ProjectName);
            Class clazz = wipProject.AddClass(ProjectHelpers.ClassName);
            wipProject.AddField<T>(ProjectHelpers.ClassName, new FieldDefinition<T>(ProjectHelpers.ValueFieldName));
            RunnableProject runnableProject = ProjectBuilder.Build(wipProject);

            dynamic? instance = runnableProject.CreateInstance(ProjectHelpers.ClassName, ProjectHelpers.InstanceName);

            Field<T>? field = instance!.GetField<T>(ProjectHelpers.ValueFieldName);
            field?.SetValue(fieldValue);

            runnableProject.SetInstanceValue<T>(ProjectHelpers.InstanceName, ProjectHelpers.ValueFieldName, fieldValue);

            string projectDefinitionFileName = ProjectStorage.SaveProjectDefinition(runnableProject);
            string projectInstancesFileName = ProjectStorage.SaveProjectInstances(runnableProject);

            RunnableProject? loadedProject = ProjectStorage.LoadProjectDefinition<RunnableProject>(projectDefinitionFileName);
            ProjectStorage.LoadProjectInstances(loadedProject!, projectInstancesFileName);

            IInstance loadedInstance = loadedProject.GetInstance(ProjectHelpers.InstanceName);

            field = loadedInstance.GetField<T>(ProjectHelpers.ValueFieldName);

            field!.Value.ShouldBe(fieldValue);


        }
    }
}