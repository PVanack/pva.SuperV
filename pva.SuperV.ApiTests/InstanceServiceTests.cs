using pva.SuperV.Api.Exceptions;
using pva.SuperV.Api.Services.Instances;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.EngineTests;
using pva.SuperV.Model;
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
            expectedInstance = new(instance!.Name, instance!.Class.Name,
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
                BuildFieldModel("IntFieldWithFormat", typeof(int), instance)
            ]);
        }

        private static FieldModel BuildFieldModel(string fieldName, Type fieldType, Instance instance)
            => new(fieldName, fieldType.ToString(), FieldValueMapper.ToDto(instance!.GetField(fieldName)));

        [Fact]
        public void SearchInstancesPaged_ShouldReturnPageOfInstances()
        {

            // Act
            InstancePagedSearchRequest search = new(1, 5, null, null, null);
            PagedSearchResult<InstanceModel> page1Result = instanceService.SearchInstances(runnableProject.GetId(), search);
            search = search with { PageNumber = 2, PageSize = 10 };
            PagedSearchResult<InstanceModel> page2Result = instanceService.SearchInstances(runnableProject.GetId(), search);
            search = search with { PageNumber = 3 };
            PagedSearchResult<InstanceModel> page3Result = instanceService.SearchInstances(runnableProject.GetId(), search);

            // Assert
            List<InstanceModel> expectedInstances = [.. runnableProject.Instances.Select(entry => InstanceMapper.ToDto(entry.Value))];

            page1Result.ShouldNotBeNull();
            page1Result.PageNumber.ShouldBe(1);
            page1Result.PageSize.ShouldBe(5);
            page1Result.Count.ShouldBe(runnableProject.Instances.Count);
            page1Result.Result.ShouldBeEquivalentTo(expectedInstances.Take(5).ToList());

            page2Result.ShouldNotBeNull();
            page2Result.PageNumber.ShouldBe(2);
            page2Result.PageSize.ShouldBe(10);
            page2Result.Count.ShouldBe(runnableProject.Instances.Count);
            page2Result.Result.ShouldBeEquivalentTo(expectedInstances.Skip(10).Take(10).ToList());

            page3Result.ShouldNotBeNull();
            page3Result.PageNumber.ShouldBe(3);
            page3Result.PageSize.ShouldBe(10);
            page3Result.Count.ShouldBe(runnableProject.Instances.Count);
            page3Result.Result.ShouldBeEmpty();
        }

        [Fact]
        public void SearchInstancesSortedByNameAsc_ShouldReturnPageOfInstancesSorted()
        {
            // Act
            InstancePagedSearchRequest search = new(1, 5, null, "name", null);
            PagedSearchResult<InstanceModel> pagedResult = instanceService.SearchInstances(runnableProject.GetId(), search);

            // Assert
            List<InstanceModel> expectedInstances = [.. runnableProject.Instances.Select(entry => InstanceMapper.ToDto(entry.Value))];
            expectedInstances.Sort(new Comparison<InstanceModel>((a, b) => a.Name.CompareTo(b.Name)));

            pagedResult.ShouldNotBeNull();
            pagedResult.PageNumber.ShouldBe(1);
            pagedResult.PageSize.ShouldBe(5);
            pagedResult.Count.ShouldBe(expectedInstances.Count);
            pagedResult.Result.ShouldBeEquivalentTo(expectedInstances.Take(5).ToList());
        }

        [Fact]
        public void SearchInstancesSortedByNameDesc_ShouldReturnPageOfInstancesSorted()
        {
            // Act
            InstancePagedSearchRequest search = new(1, 5, null, "-name", null);
            PagedSearchResult<InstanceModel> pagedResult = instanceService.SearchInstances(runnableProject.GetId(), search);

            // Assert
            List<InstanceModel> expectedInstances = [.. runnableProject.Instances.Select(entry => InstanceMapper.ToDto(entry.Value))];
            expectedInstances.Sort(new Comparison<InstanceModel>((a, b) => a.Name.CompareTo(b.Name)));
            expectedInstances.Reverse();

            pagedResult.ShouldNotBeNull();
            pagedResult.PageNumber.ShouldBe(1);
            pagedResult.PageSize.ShouldBe(5);
            pagedResult.Count.ShouldBe(expectedInstances.Count);
            pagedResult.Result.ShouldBeEquivalentTo(expectedInstances.Take(5).ToList());
        }

        [Fact]
        public void SearchInstancesSortedWithInvalidOption_ShouldThrowException()
        {
            // Act
            InstancePagedSearchRequest search = new(1, 5, null, "-InvalidOption", null);
            Assert.Throws<InvalidSortOptionException>(() => instanceService.SearchInstances(runnableProject.GetId(), search));
        }

        [Fact]
        public void SearchInstancesByName_ShouldReturnPageOfInstances()
        {

            // Act
            InstancePagedSearchRequest search = new(1, 5, "IntField", null, null);
            PagedSearchResult<InstanceModel> pagedResult = instanceService.SearchInstances(runnableProject.GetId(), search);

            // Assert
            List<InstanceModel> expectedInstances = [.. runnableProject.Instances.Values
                .Where(clazz=> clazz.Name!.Contains("IntField"))
                .Select(clazz => InstanceMapper.ToDto(clazz))];

            pagedResult.ShouldNotBeNull();
            pagedResult.PageNumber.ShouldBe(1);
            pagedResult.PageSize.ShouldBe(5);
            pagedResult.Count.ShouldBe(runnableProject.Instances.Count);
            pagedResult.Result.ShouldBeEquivalentTo(expectedInstances.Take(5).ToList());
        }

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
