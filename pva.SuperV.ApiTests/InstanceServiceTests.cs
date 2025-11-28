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
            instanceService = new(LoggerFactory);
            fieldValueService = new(LoggerFactory);
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
        public async Task SearchInstancesPaged_ShouldReturnPageOfInstances()
        {

            // Act
            InstancePagedSearchRequest search = new(1, 5, null, null, null);
            PagedSearchResult<InstanceModel> page1Result = await instanceService.SearchInstancesAsync(runnableProject.GetId(), search);
            search = search with { PageNumber = 2, PageSize = 10 };
            PagedSearchResult<InstanceModel> page2Result = await instanceService.SearchInstancesAsync(runnableProject.GetId(), search);
            search = search with { PageNumber = 3 };
            PagedSearchResult<InstanceModel> page3Result = await instanceService.SearchInstancesAsync(runnableProject.GetId(), search);

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
        public async Task SearchInstancesSortedByNameAsc_ShouldReturnPageOfInstancesSorted()
        {
            // Act
            InstancePagedSearchRequest search = new(1, 5, null, "name", null);
            PagedSearchResult<InstanceModel> pagedResult = await instanceService.SearchInstancesAsync(runnableProject.GetId(), search);

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
        public async Task SearchInstancesSortedByNameDesc_ShouldReturnPageOfInstancesSorted()
        {
            // Act
            InstancePagedSearchRequest search = new(1, 5, null, "-name", null);
            PagedSearchResult<InstanceModel> pagedResult = await instanceService.SearchInstancesAsync(runnableProject.GetId(), search);

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
        public async Task SearchInstancesSortedWithInvalidOption_ShouldThrowException()
        {
            // Act
            InstancePagedSearchRequest search = new(1, 5, null, "-InvalidOption", null);
            await Assert.ThrowsAsync<InvalidSortOptionException>(async () => await instanceService.SearchInstancesAsync(runnableProject.GetId(), search));
        }

        [Fact]
        public async Task SearchInstancesByName_ShouldReturnPageOfInstances()
        {

            // Act
            InstancePagedSearchRequest search = new(1, 5, "IntField", null, null);
            PagedSearchResult<InstanceModel> pagedResult = await instanceService.SearchInstancesAsync(runnableProject.GetId(), search);

            // Assert
            List<InstanceModel> expectedInstances = [.. runnableProject.Instances.Values
                .Where(instance => instance.Name!.Contains("IntField"))
                .Select(instance => InstanceMapper.ToDto(instance))];

            pagedResult.ShouldNotBeNull();
            pagedResult.PageNumber.ShouldBe(1);
            pagedResult.PageSize.ShouldBe(5);
            pagedResult.Count.ShouldBe(runnableProject.Instances.Count);
            pagedResult.Result.ShouldBeEquivalentTo(expectedInstances.Take(5).ToList());
        }

        [Fact]
        public async Task SearchInstancesByClassName_ShouldReturnPageOfInstances()
        {
            _ = runnableProject.CreateInstance(ClassName, "DummyInstance");
            _ = runnableProject.CreateInstance(BaseClassName, "BaseDummyInstance");
            // Act
            InstancePagedSearchRequest search = new(1, 5, null, null, BaseClassName);
            PagedSearchResult<InstanceModel> pagedResult = await instanceService.SearchInstancesAsync(runnableProject.GetId(), search);

            // Assert
            List<InstanceModel> expectedInstances = [.. runnableProject.Instances.Values
                .Where(instance => instance.Class.Name.Equals(BaseClassName)
                                    || instance.Class.BaseClass?.Name.Equals(BaseClassName) == true)
                .Select(instance => InstanceMapper.ToDto(instance))];

            pagedResult.ShouldNotBeNull();
            pagedResult.PageNumber.ShouldBe(1);
            pagedResult.PageSize.ShouldBe(5);
            pagedResult.Count.ShouldBe(runnableProject.Instances.Count);
            pagedResult.Result.ShouldBeEquivalentTo(expectedInstances.Take(5).ToList());
        }

        [Fact]
        public async Task GetInstances_ShouldReturnListOfInstances()
        {
            List<InstanceModel> expectedInstances = [expectedInstance];
            // Act
            var result = await instanceService.GetInstancesAsync(runnableProject.GetId());

            // Assert
            result.ShouldBeEquivalentTo(expectedInstances);
        }

        [Fact]
        public async Task GetInstance_ShouldReturnInstance_WhenInstanceExists()
        {
            // Act
            var result = await instanceService.GetInstanceAsync(runnableProject.GetId(), InstanceName);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEquivalentTo(expectedInstance);
        }

        [Fact]
        public async Task GetInstance_ShouldThrowUnknownEntityException_WhenInstanceDoesNotExist()
        {
            // Act & Assert
            await Assert.ThrowsAsync<UnknownEntityException>(async ()
                => await instanceService.GetInstanceAsync(runnableProject.GetId(), "UnknownInstance"));
        }

        [Fact]
        public async Task CreateInstanceInRunnableProject_ShouldCreateInstance()
        {
            InstanceModel expectedCreatedInstance = expectedInstance with { Name = "Instance1" };
            // Act & Assert
            InstanceModel createInstanceModel = await instanceService.CreateInstanceAsync(runnableProject.GetId(), expectedCreatedInstance with { Fields = [] });

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
                }
                );
            }
            expectedCreatedInstance = expectedCreatedInstance with { Fields = fieldsUpdatedWithTimestampsAndQualities };
            createInstanceModel.ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedCreatedInstance);
        }

        [Fact]
        public async Task CreateInstanceWithInitialedFieldsInRunnableProject_ShouldCreateInstanceWithFieldsInitialized()
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
            InstanceModel createInstanceModel = await instanceService.CreateInstanceAsync(runnableProject.GetId(),
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
        public async Task GetInstanceField_ShouldReturnField()
        {
            // Act
            FieldModel expectedField = expectedInstance.Fields[0];
            FieldModel retrievedField = await fieldValueService.GetFieldAsync(runnableProject.GetId(), expectedInstance.Name, expectedField.Name);

            // Assert
            retrievedField.ShouldBeEquivalentTo(expectedField);
        }
    }
}
