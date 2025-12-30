using pva.SuperV.Api.Exceptions;
using pva.SuperV.Api.Services.FieldDefinitions;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.EngineTests;
using pva.SuperV.Model;
using pva.SuperV.Model.FieldDefinitions;
using Shouldly;

namespace pva.SuperV.ApiTests
{
    [Collection("Project building")]
    public class FieldDefinitionServiceTests : SuperVTestsBase
    {
        const string className = "DummyClass";
        private readonly FieldDefinitionService fieldDefinitionService;
        private readonly RunnableProject runnableProject;
        private readonly WipProject wipProject;
        private readonly List<FieldDefinitionModel> allFieldsExpectedFieldDefinitions =
            [
                new BoolFieldDefinitionModel("BoolField", default, null),
                new DateTimeFieldDefinitionModel("DateTimeField", default, null),
                new DoubleFieldDefinitionModel("DoubleField", default, null),
                new FloatFieldDefinitionModel("FloatField", default, null),
                new IntFieldDefinitionModel("IntField", default, null),
                new LongFieldDefinitionModel("LongField", default, null),
                new ShortFieldDefinitionModel("ShortField", default, null),
                new StringFieldDefinitionModel("StringField", default, null),
                new TimeSpanFieldDefinitionModel("TimeSpanField", TimeSpan.FromDays(0), null),
                new UintFieldDefinitionModel("UintField", default, null),
                new UlongFieldDefinitionModel("UlongField", default, null),
                new UshortFieldDefinitionModel("UshortField", default, null),
                new IntFieldDefinitionModel("IntFieldWithFormat", default, "AlarmStates")
            ];

        public FieldDefinitionServiceTests()
        {
            fieldDefinitionService = new(LoggerFactory);
            runnableProject = CreateRunnableProject();
            wipProject = Project.CreateProject(runnableProject);
        }

        [Fact]
        public async Task GetClassFieldDefinitions_ShouldReturnListOfClassFieldDefinitions()
        {
            // GIVEN

            // WHEN
            List<FieldDefinitionModel> fieldDefinitions = await fieldDefinitionService.GetFieldsAsync(runnableProject.GetId(), AllFieldsClassName);

            // THEN
            fieldDefinitions
                .ShouldNotBeNull()
                .ShouldBeEquivalentTo(allFieldsExpectedFieldDefinitions);
        }

        [Fact]
        public async Task SearchPagedFieldDefinitions_ShouldReturnListOfPagedFieldDefinitions()
        {
            // GIVEN
            Class clazz = CreateDummyFieldDefinitions(className);

            // Act
            FieldDefinitionPagedSearchRequest search = new(1, 5, null, null);
            PagedSearchResult<FieldDefinitionModel> page1Result = await fieldDefinitionService.SearchFieldsAsync(wipProject.GetId(), className, search);
            search = search with { PageNumber = 2, PageSize = 10 };
            PagedSearchResult<FieldDefinitionModel> page2Result = await fieldDefinitionService.SearchFieldsAsync(wipProject.GetId(), className, search);
            search = search with { PageNumber = 3 };
            PagedSearchResult<FieldDefinitionModel> page3Result = await fieldDefinitionService.SearchFieldsAsync(wipProject.GetId(), className, search);
            search = search with { PageNumber = 4 };
            PagedSearchResult<FieldDefinitionModel> page4Result = await fieldDefinitionService.SearchFieldsAsync(wipProject.GetId(), className, search);

            // Assert
            List<FieldDefinitionModel> expectedFieldDefinitions = [.. clazz.FieldDefinitions.Select(entry => FieldDefinitionMapper.ToDto(entry.Value))];

            page1Result.ShouldNotBeNull();
            page1Result.PageNumber.ShouldBe(1);
            page1Result.PageSize.ShouldBe(5);
            page1Result.Count.ShouldBe(clazz.FieldDefinitions.Count);
            page1Result.Result.ShouldBeEquivalentTo(expectedFieldDefinitions.Take(5).ToList());

            page2Result.ShouldNotBeNull();
            page2Result.PageNumber.ShouldBe(2);
            page2Result.PageSize.ShouldBe(10);
            page2Result.Count.ShouldBe(clazz.FieldDefinitions.Count);
            page2Result.Result.ShouldBeEquivalentTo(expectedFieldDefinitions.Skip(10).Take(10).ToList());

            page3Result.ShouldNotBeNull();
            page3Result.PageNumber.ShouldBe(3);
            page3Result.PageSize.ShouldBe(10);
            page3Result.Count.ShouldBe(clazz.FieldDefinitions.Count);
            page3Result.Result.ShouldBeEquivalentTo(expectedFieldDefinitions.Skip(20).Take(10).ToList());

            page4Result.ShouldNotBeNull();
            page4Result.PageNumber.ShouldBe(4);
            page4Result.PageSize.ShouldBe(10);
            page4Result.Count.ShouldBe(clazz.FieldDefinitions.Count);
            page4Result.Result.ShouldBeEmpty();
        }

        [Fact]
        public async Task SearchClassesSortedByNameAsc_ShouldReturnPageOfClassesSorted()
        {
            // GIVEN
            Class clazz = CreateDummyFieldDefinitions(className);

            // Act
            FieldDefinitionPagedSearchRequest search = new(1, 5, null, "name");
            PagedSearchResult<FieldDefinitionModel> pagedResult = await fieldDefinitionService.SearchFieldsAsync(wipProject.GetId(), className, search);

            // Assert
            List<FieldDefinitionModel> expectedFieldDefinitions = [.. clazz.FieldDefinitions.Select(entry => FieldDefinitionMapper.ToDto(entry.Value))];
            expectedFieldDefinitions.Sort(new Comparison<FieldDefinitionModel>((a, b) => a.Name.CompareTo(b.Name)));

            pagedResult.ShouldNotBeNull();
            pagedResult.PageNumber.ShouldBe(1);
            pagedResult.PageSize.ShouldBe(5);
            pagedResult.Count.ShouldBe(expectedFieldDefinitions.Count);
            pagedResult.Result.ShouldBeEquivalentTo(expectedFieldDefinitions.Take(5).ToList());
        }

        [Fact]
        public async Task SearchClassesSortedByNameDesc_ShouldReturnPageOfClassesSorted()
        {
            // GIVEN
            Class clazz = CreateDummyFieldDefinitions(className);

            // Act
            FieldDefinitionPagedSearchRequest search = new(1, 5, null, "-name");
            PagedSearchResult<FieldDefinitionModel> pagedResult = await fieldDefinitionService.SearchFieldsAsync(wipProject.GetId(), className, search);

            // Assert
            List<FieldDefinitionModel> expectedFieldDefinitions = [.. clazz.FieldDefinitions.Select(entry => FieldDefinitionMapper.ToDto(entry.Value))];
            expectedFieldDefinitions.Sort(new Comparison<FieldDefinitionModel>((a, b) => a.Name.CompareTo(b.Name)));
            expectedFieldDefinitions.Reverse();

            pagedResult.ShouldNotBeNull();
            pagedResult.PageNumber.ShouldBe(1);
            pagedResult.PageSize.ShouldBe(5);
            pagedResult.Count.ShouldBe(expectedFieldDefinitions.Count);
            pagedResult.Result.ShouldBeEquivalentTo(expectedFieldDefinitions.Take(5).ToList());
        }

        [Fact]
        public async Task SearchClassesSortedWithInvalidOption_ShouldThrowException()
        {
            // GIVEN
            _ = CreateDummyFieldDefinitions(className);

            // Act
            FieldDefinitionPagedSearchRequest search = new(1, 5, null, "-InvalidOption");
            await Assert.ThrowsAsync<InvalidSortOptionException>(() => fieldDefinitionService.SearchFieldsAsync(wipProject.GetId(), className, search));
        }

        [Fact]
        public async Task SearchClassesByName_ShouldReturnPageOfClasses()
        {
            // GIVEN
            Class clazz = CreateDummyFieldDefinitions(className);

            // Act
            FieldDefinitionPagedSearchRequest search = new(1, 5, "DummyField1", null);
            PagedSearchResult<FieldDefinitionModel> pagedResult = await fieldDefinitionService.SearchFieldsAsync(wipProject.GetId(), className, search);

            // Assert
            List<FieldDefinitionModel> expectedFieldDefinition = [.. clazz.FieldDefinitions.Values
                .Where(fieldDefinition => fieldDefinition.Name!.Contains("DummyField1"))
                .Select(fieldDefinition => FieldDefinitionMapper.ToDto(fieldDefinition))];

            pagedResult.ShouldNotBeNull();
            pagedResult.PageNumber.ShouldBe(1);
            pagedResult.PageSize.ShouldBe(5);
            pagedResult.Count.ShouldBe(clazz.FieldDefinitions.Count);
            pagedResult.Result.ShouldBeEquivalentTo(expectedFieldDefinition.Take(5).ToList());
        }

        private Class CreateDummyFieldDefinitions(string className)
        {
            Class clazz = wipProject.AddClass(className);
            for (int i = 0; i < 10; i++)
            {
                clazz.AddField(new FieldDefinition<int>($"DummyField{i + 1}"));
            }
            return clazz;
        }

        [Fact]
        public async Task GetClassFieldDefinition_ShouldReturnClassFieldDefinition()
        {
            // GIVEN
            FieldDefinitionModel expectedFieldDefinition =
                new StringFieldDefinitionModel(BaseClassFieldName, "InheritedField", null);
            // WHEN
            FieldDefinitionModel fieldDefinition = await fieldDefinitionService.GetFieldAsync(runnableProject.GetId(), BaseClassName, BaseClassFieldName);

            // THEN
            fieldDefinition
                .ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedFieldDefinition);
        }

        [Fact]
        public async Task CreateClassFieldDefinition_ShouldCreateClassFieldDefinition()
        {
            // GIVEN
            List<FieldDefinitionModel> expectedFieldDefinitions = allFieldsExpectedFieldDefinitions;

            // WHEN
            List<FieldDefinitionModel> createdFieldDefinitions = await fieldDefinitionService.CreateFieldsAsync(wipProject.GetId(), BaseClassName, expectedFieldDefinitions);

            // THEN
            createdFieldDefinitions.ShouldBeEquivalentTo(expectedFieldDefinitions);
        }

        [Fact]
        public async Task UpdateClassFieldDefinition_ShouldUpdateClassFieldDefinition()
        {
            // GIVEN
            FieldDefinitionModel expectedFieldDefinition = new IntFieldDefinitionModel("IntFieldWithFormat", default, null);

            // WHEN
            FieldDefinitionModel updatedFieldDefinition = await fieldDefinitionService.UpdateFieldAsync(wipProject.GetId(), AllFieldsClassName, expectedFieldDefinition.Name, expectedFieldDefinition);

            // THEN
            updatedFieldDefinition.ShouldBeEquivalentTo(expectedFieldDefinition);
        }

        [Fact]
        public async Task UpdateClassFieldDefinitionChangingType_ShouldThrowException()
        {
            // GIVEN
            FieldDefinitionModel expectedFieldDefinition = new ShortFieldDefinitionModel("IntFieldWithFormat", default, null);

            // WHEN/THEN
            await Assert.ThrowsAsync<WrongFieldTypeException>(()
                => fieldDefinitionService.UpdateFieldAsync(wipProject.GetId(), AllFieldsClassName, expectedFieldDefinition.Name, expectedFieldDefinition));
        }

        [Fact]
        public async Task UpdateClassFieldDefinitionChangingName_ShouldThrowException()
        {
            // GIVEN
            FieldDefinitionModel expectedFieldDefinition = new IntFieldDefinitionModel("ChangedName", default, null);

            // WHEN/THEN
            await Assert.ThrowsAsync<EntityPropertyNotChangeableException>(()
                => fieldDefinitionService.UpdateFieldAsync(wipProject.GetId(), AllFieldsClassName, "IntFieldWithFormat", expectedFieldDefinition));
        }

        [Fact]
        public async Task DeleteClassFieldDefinition_ShouldDeleteClassFieldDefinition()
        {
            // GIVEN

            // WHEN
            await fieldDefinitionService.DeleteFieldAsync(wipProject.GetId(), BaseClassName, BaseClassFieldName);

            // THEN
            wipProject.GetClass(BaseClassName).FieldDefinitions.Keys.ShouldNotContain(BaseClassFieldName);
        }
    }
}
