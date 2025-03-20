using pva.SuperV.Api.Services.Instances;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.EngineTests;
using pva.SuperV.Model.Instances;
using Shouldly;

namespace pva.SuperV.ApiTests
{
    [Collection("Project building")]
    public class InstanceServiceTests : SuperVTestsBase
    {
        private readonly InstanceService instanceService;
        private readonly FieldValueService fieldValueService;
        private readonly RunnableProject runnableProject;
        private readonly InstanceModel expectedInstance;

        public InstanceServiceTests()
        {
            instanceService = new();
            fieldValueService = new();
            runnableProject = CreateRunnableProject();
            Instance? instance = runnableProject.CreateInstance(AllFieldsClassName, InstanceName);
            expectedInstance = new(instance!.Name, instance!.Class.Name!,
            [
            BuildFieldModel("BoolField", typeof(bool), instance),
            BuildFieldModel("DateTimeField", typeof(DateTime), instance),
            BuildFieldModel("DoubleField", typeof(double), instance),
            BuildFieldModel("FloatField", typeof(float), instance),
            BuildFieldModel("IntField", typeof(int), instance),
            BuildFieldModel("LongField", typeof(long), instance),
            BuildFieldModel("ShortField", typeof(short), instance),
            BuildFieldModel("StringField", typeof(string), instance),
            BuildFieldModel("TimeSpanField", typeof(TimeSpan), instance),
            BuildFieldModel("UintField", typeof(uint), instance),
            BuildFieldModel("UlongField", typeof(ulong), instance),
            BuildFieldModel("UshortField", typeof(ushort), instance),
            ]);
        }

        private static FieldModel BuildFieldModel(string fieldName, Type fieldType, Instance instance)
            => new FieldModel(fieldName, fieldType.ToString(), FieldValueMapper.ToDto(instance!.GetField(fieldName)));

        [Fact]
        public void GetInstances_ShouldReturnListOfInstances()
        {
            List<InstanceModel> expectedInstances = [expectedInstance];
            // Act
            var result = instanceService.GetInstances(runnableProject.GetId());

            // Assert
            result.ShouldBeEquivalentTo(expectedInstances);
        }

        [Fact]
        public void GetInstance_ShouldReturnInstance_WhenInstanceExists()
        {
            // Act
            var result = instanceService.GetInstance(runnableProject.GetId(), InstanceName);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEquivalentTo(expectedInstance);
        }

        [Fact]
        public void GetInstance_ShouldThrowUnknownEntityException_WhenInstanceDoesNotExist()
        {
            // Act & Assert
            Assert.Throws<UnknownEntityException>(()
                => instanceService.GetInstance(runnableProject.GetId(), "UnknownInstance"));
        }

        [Fact]
        public void CreateInstanceInRunnableProject_ShouldCreateInstance()
        {
            InstanceModel expectedCreatedInstance = expectedInstance with { Name = "Instance1" };
            // Act & Assert
            InstanceModel createInstanceModel = instanceService.CreateInstance(runnableProject.GetId(), expectedCreatedInstance with { Fields = [] });

            createInstanceModel.Fields.Count.ShouldBe(expectedCreatedInstance.Fields.Count);
            List<FieldModel> fieldsUpdatedWithTimestampsAndQualities = [];
            for (int index = 0; index < createInstanceModel.Fields.Count; index++)
            {
                fieldsUpdatedWithTimestampsAndQualities.Add(expectedCreatedInstance.Fields[index]
                    with
                {
                    FieldValue = expectedCreatedInstance.Fields[index].FieldValue
                            with
                    {
                        Quality = createInstanceModel.Fields[index].FieldValue.Quality,
                        Timestamp = createInstanceModel.Fields[index].FieldValue.Timestamp
                    }
                }
                );
            }
            expectedCreatedInstance = expectedCreatedInstance with { Fields = fieldsUpdatedWithTimestampsAndQualities };
            createInstanceModel.ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedCreatedInstance);
        }

        [Fact]
        public void CreateInstanceWithInitialedFieldsInRunnableProject_ShouldCreateInstanceWithFieldsInitialized()
        {
            InstanceModel expectedCreatedInstance = expectedInstance with
            {
                Name = "Instance1",
                Fields = [.. expectedInstance.Fields
                            .Select(field
                                => field.Name.Equals(expectedInstance.Fields[0].Name)
                                        ? field with { FieldValue = new BoolFieldValueModel(false, null, QualityLevel.Bad, DateTime.Now) }
                                        : field)]
            };
            // Act & Assert
            InstanceModel createInstanceModel = instanceService.CreateInstance(runnableProject.GetId(),
                expectedCreatedInstance with
                {
                    Fields = [expectedCreatedInstance.Fields[0]]
                });

            createInstanceModel.Fields.Count.ShouldBe(expectedCreatedInstance.Fields.Count);
            List<FieldModel> fieldsUpdatedWithTimestampsAndQualities = [];
            for (int index = 0; index < createInstanceModel.Fields.Count; index++)
            {
                fieldsUpdatedWithTimestampsAndQualities.Add(expectedCreatedInstance.Fields[index] with
                {
                    FieldValue = expectedCreatedInstance.Fields[index].FieldValue with
                    {
                        Quality = createInstanceModel.Fields[index].FieldValue.Quality,
                        Timestamp = createInstanceModel.Fields[index].FieldValue.Timestamp
                    }
                });
            }
            expectedCreatedInstance = expectedCreatedInstance with { Fields = fieldsUpdatedWithTimestampsAndQualities };
            createInstanceModel.ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedCreatedInstance);
        }

        [Fact]
        public void GetInstanceField_ShouldReturnField()
        {
            // Act
            FieldModel expectedField = expectedInstance.Fields[0];
            FieldModel retrievedField = fieldValueService.GetField(runnableProject.GetId(), expectedInstance.Name, expectedField.Name);

            // Assert
            retrievedField.ShouldBeEquivalentTo(expectedField);
        }
    }
}
