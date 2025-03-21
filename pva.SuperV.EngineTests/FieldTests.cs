using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using Shouldly;

namespace pva.SuperV.EngineTests
{
    [Collection("Project building")]
    public class FieldTests : SuperVTestsBase
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
        public void CheckValueIsAsExpectedForBoolField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<bool>(true);
        }

        [Fact]
        public void CheckValueIsAsExpectedForDateTimeField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<DateTime>(DateTime.Now);
        }

        [Fact]
        public void CheckValueIsAsExpectedForDoubleField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<double>(1234.4321);
        }

        [Fact]
        public void CheckValueIsAsExpectedForFloatField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<float>(1234.4321f);
        }

        [Fact]
        public void CheckValueIsAsExpectedForIntField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<int>(-1234);
        }

        [Fact]
        public void CheckValueIsAsExpectedForLongField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<long>(-1234);
        }

        [Fact]
        public void CheckValueIsAsExpectedForShortField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<short>(-1234);
        }

        [Fact]
        public void CheckValueIsAsExpectedForStringField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<string>("Some text");
        }

        [Fact]
        public void CheckValueIsAsExpectedForTimeSpanField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<TimeSpan>(TimeSpan.FromMinutes(1.0));
        }

        [Fact]
        public void CheckValueIsAsExpectedForUintField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<uint>(1234);
        }

        [Fact]
        public void CheckValueIsAsExpectedForUlongField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<ulong>(1234);
        }

        [Fact]
        public void CheckValueIsAsExpectedForUshortField()
        {
            GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<ushort>(1234);
        }

        private static void GivenProjectWithSpecificField_WhenBuildingAndReloadingProject_ThenFieldValueIsAsExpected<T>(T fieldValue)
        {
            WipProject wipProject = Project.CreateProject(ProjectName);
            _ = wipProject.AddClass(ClassName);
            wipProject.AddField(ClassName, new FieldDefinition<T>(ValueFieldName));
            RunnableProject runnableProject = Task.Run(async () => await Project.BuildAsync(wipProject)).Result;

            dynamic? instance = runnableProject.CreateInstance(ClassName, InstanceName);

            Field<T>? field = instance!.GetField<T>(ValueFieldName);
            field?.SetValue(fieldValue);

            runnableProject.SetInstanceValue<T>(InstanceName, ValueFieldName, fieldValue);

            string projectDefinitionFileName = ProjectStorage.SaveProjectDefinition(runnableProject);
            string projectInstancesFileName = ProjectStorage.SaveProjectInstances(runnableProject);

            RunnableProject? loadedProject = ProjectStorage.LoadProjectDefinition<RunnableProject>(projectDefinitionFileName);
            ProjectStorage.LoadProjectInstances(loadedProject!, projectInstancesFileName);

            Instance? loadedInstance = loadedProject!.GetInstance(InstanceName);

            field = loadedInstance!.GetField<T>(ValueFieldName);

            field!.Value.ShouldBe(fieldValue);
        }
    }
}