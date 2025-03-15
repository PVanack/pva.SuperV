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
        private readonly InstanceService _instanceService;
        private readonly FieldValueService _fieldValueService;
        private readonly RunnableProject runnableProject;
        private readonly InstanceModel expectedInstance;

        public InstanceServiceTests()
        {
            _instanceService = new();
            _fieldValueService = new();
            runnableProject = CreateRunnableProject();
            Instance? instance = runnableProject.CreateInstance(ClassName, InstanceName);
            expectedInstance = new(instance!.Name, instance!.Class.Name!,
            [
                new FieldModel(BaseClassFieldName, typeof(string).ToString(), FieldValueMapper.ToDto(instance!.GetField(BaseClassFieldName))),
                new FieldModel(ValueFieldName, typeof(int).ToString(), FieldValueMapper.ToDto(instance!.GetField(ValueFieldName))),
                new FieldModel(HighHighLimitFieldName, typeof(int).ToString(), FieldValueMapper.ToDto(instance!.GetField(HighHighLimitFieldName))),
                new FieldModel(HighLimitFieldName, typeof(int).ToString(), FieldValueMapper.ToDto(instance!.GetField(HighLimitFieldName))),
                new FieldModel(LowLimitFieldName, typeof(int).ToString(), FieldValueMapper.ToDto(instance!.GetField(LowLimitFieldName))),
                new FieldModel(LowLowLimitFieldName, typeof(int).ToString(), FieldValueMapper.ToDto(instance!.GetField(LowLowLimitFieldName))),
                new FieldModel(AlarmStateFieldName, typeof(int).ToString(), FieldValueMapper.ToDto(instance!.GetField(AlarmStateFieldName)))
            ]);
        }

        [Fact]
        public void GetInstances_ShouldReturnListOfInstances()
        {
            List<InstanceModel> expectedInstances = [expectedInstance];
            // Act
            var result = _instanceService.GetInstances(runnableProject.GetId());

            // Assert
            result.ShouldBeEquivalentTo(expectedInstances);
        }

        [Fact]
        public void GetInstance_ShouldReturnInstance_WhenInstanceExists()
        {
            // Act
            var result = _instanceService.GetInstance(runnableProject.GetId(), InstanceName);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEquivalentTo(expectedInstance);
        }

        [Fact]
        public void GetInstance_ShouldThrowUnknownEntityException_WhenInstanceDoesNotExist()
        {
            // Act & Assert
            Assert.Throws<UnknownEntityException>(()
                => _instanceService.GetInstance(runnableProject.GetId(), "UnknownInstance"));
        }

        [Fact]
        public void CreateInstanceInRunnableProject_ShouldCreateInstance()
        {
            InstanceModel expectedCreatedInstance = expectedInstance with { Name = "Instance1" };
            // Act & Assert
            InstanceModel createInstanceModel = _instanceService.CreateInstance(runnableProject.GetId(), expectedCreatedInstance with { Fields = [] });

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
                                        ? field with { FieldValue = new StringFieldValueModel("ExpectedString", QualityLevel.Bad, DateTime.Now) }
                                        : field)]
            };
            // Act & Assert
            InstanceModel createInstanceModel = _instanceService.CreateInstance(runnableProject.GetId(),
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
            FieldModel retrievedField = _fieldValueService.GetField(runnableProject.GetId(), expectedInstance.Name, expectedField.Name);

            // Assert
            retrievedField.ShouldBeEquivalentTo(expectedField);
        }

        [Fact]
        public void UpdateInstanceFieldValueInRunnableProject_ShouldUpdateInstaceFieldValue()
        {
            StringFieldValueModel expectedFieldValue = ((StringFieldValueModel)expectedInstance.Fields[0].FieldValue) with { Value = "The value has been updated" };
            // Act & Assert
            FieldValueModel updatedFieldModel = _fieldValueService.UpdateFieldValue(runnableProject.GetId(), expectedInstance.Name, expectedInstance.Fields[0].Name, expectedFieldValue);

            updatedFieldModel.ShouldNotBeNull()
                .ShouldBeOfType<StringFieldValueModel>()
                .ShouldBeEquivalentTo(expectedFieldValue);
        }
    }
}
