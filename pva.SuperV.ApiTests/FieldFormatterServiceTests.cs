using pva.SuperV.Api.Exceptions;
using pva.SuperV.Api.Services.FieldFormatters;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.FieldFormatters;
using pva.SuperV.EngineTests;
using pva.SuperV.Model;
using pva.SuperV.Model.FieldFormatters;
using Shouldly;

namespace pva.SuperV.ApiTests
{
    [Collection("Project building")]
    public class FieldFormatterServiceTests : SuperVTestsBase
    {
        private readonly FieldFormatterService fieldFormatterService;
        private readonly RunnableProject runnableProject;
        private readonly WipProject wipProject;

        public FieldFormatterServiceTests()
        {
            fieldFormatterService = new();
            runnableProject = CreateRunnableProject();
            wipProject = Project.CreateProject(runnableProject);
        }

        [Fact]
        public async Task GetFieldFormatterTypes_ShouldReturnListOfFieldFormatterTypes()
        {
            // GIVEN
            // WHEN
            List<string> formatterTypes = await fieldFormatterService.GetFieldFormatterTypesAsync();

            // THEN
            formatterTypes
                .ShouldNotBeNull()
                .ShouldContain(typeof(EnumFormatter).ToString());
        }

        [Fact]
        public async Task SearchClassesPaged_ShouldReturnPageOfClasses()
        {
            CreateDummyFieldFormatters();

            // Act
            FieldFormatterPagedSearchRequest search = new(1, 5, null, null);
            PagedSearchResult<FieldFormatterModel> page1Result = await fieldFormatterService.SearchFieldFormattersAsync(wipProject.GetId(), search);
            search = search with { PageNumber = 2, PageSize = 10 };
            PagedSearchResult<FieldFormatterModel> page2Result = await fieldFormatterService.SearchFieldFormattersAsync(wipProject.GetId(), search);
            search = search with { PageNumber = 3 };
            PagedSearchResult<FieldFormatterModel> page3Result = await fieldFormatterService.SearchFieldFormattersAsync(wipProject.GetId(), search);
            search = search with { PageNumber = 4 };
            PagedSearchResult<FieldFormatterModel> page4Result = await fieldFormatterService.SearchFieldFormattersAsync(wipProject.GetId(), search);

            // Assert
            List<FieldFormatterModel> expectedFieldFormatters = [.. wipProject.FieldFormatters.Select(entry => FieldFormatterMapper.ToDto(entry.Value))];

            page1Result.ShouldNotBeNull();
            page1Result.PageNumber.ShouldBe(1);
            page1Result.PageSize.ShouldBe(5);
            page1Result.Count.ShouldBe(wipProject.FieldFormatters.Count);
            page1Result.Result.ShouldBeEquivalentTo(expectedFieldFormatters.Take(5).ToList());

            page2Result.ShouldNotBeNull();
            page2Result.PageNumber.ShouldBe(2);
            page2Result.PageSize.ShouldBe(10);
            page2Result.Count.ShouldBe(wipProject.FieldFormatters.Count);
            page2Result.Result.ShouldBeEquivalentTo(expectedFieldFormatters.Skip(10).Take(10).ToList());

            page3Result.ShouldNotBeNull();
            page3Result.PageNumber.ShouldBe(3);
            page3Result.PageSize.ShouldBe(10);
            page3Result.Count.ShouldBe(wipProject.FieldFormatters.Count);
            page3Result.Result.ShouldBeEquivalentTo(expectedFieldFormatters.Skip(20).Take(10).ToList());

            page4Result.ShouldNotBeNull();
            page4Result.PageNumber.ShouldBe(4);
            page4Result.PageSize.ShouldBe(10);
            page4Result.Count.ShouldBe(wipProject.FieldFormatters.Count);
            page4Result.Result.ShouldBeEmpty();
        }

        [Fact]
        public async Task SearchClassesSortedByNameAsc_ShouldReturnPageOfFieldFormattersSorted()
        {
            CreateDummyFieldFormatters();

            // Act
            FieldFormatterPagedSearchRequest search = new(1, 5, null, "name");
            PagedSearchResult<FieldFormatterModel> pagedResult = await fieldFormatterService.SearchFieldFormattersAsync(wipProject.GetId(), search);

            // Assert
            List<FieldFormatterModel> expectedFieldFormatters = [.. wipProject.FieldFormatters.Select(entry => FieldFormatterMapper.ToDto(entry.Value))];
            expectedFieldFormatters.Sort(new Comparison<FieldFormatterModel>((a, b) => a.Name.CompareTo(b.Name)));

            pagedResult.ShouldNotBeNull();
            pagedResult.PageNumber.ShouldBe(1);
            pagedResult.PageSize.ShouldBe(5);
            pagedResult.Count.ShouldBe(expectedFieldFormatters.Count);
            pagedResult.Result.ShouldBeEquivalentTo(expectedFieldFormatters.Take(5).ToList());
        }

        [Fact]
        public async Task SearchFieldFormattersSortedByNameDesc_ShouldReturnPageOfFieldFormattersSorted()
        {
            CreateDummyFieldFormatters();

            // Act
            FieldFormatterPagedSearchRequest search = new(1, 5, null, "-name");
            PagedSearchResult<FieldFormatterModel> pagedResult = await fieldFormatterService.SearchFieldFormattersAsync(wipProject.GetId(), search);

            // Assert
            List<FieldFormatterModel> expectedFieldFormatters = [.. wipProject.FieldFormatters.Select(entry => FieldFormatterMapper.ToDto(entry.Value))];
            expectedFieldFormatters.Sort(new Comparison<FieldFormatterModel>((a, b) => a.Name.CompareTo(b.Name)));
            expectedFieldFormatters.Reverse();

            pagedResult.ShouldNotBeNull();
            pagedResult.PageNumber.ShouldBe(1);
            pagedResult.PageSize.ShouldBe(5);
            pagedResult.Count.ShouldBe(expectedFieldFormatters.Count);
            pagedResult.Result.ShouldBeEquivalentTo(expectedFieldFormatters.Take(5).ToList());
        }

        [Fact]
        public async Task SearchFieldFormattersSortedWithInvalidOption_ShouldThrowException()
        {
            // Act
            FieldFormatterPagedSearchRequest search = new(1, 5, null, "-InvalidOption");
            await Assert.ThrowsAsync<InvalidSortOptionException>(async () => await fieldFormatterService.SearchFieldFormattersAsync(wipProject.GetId(), search));
        }

        [Fact]
        public async Task SearchFieldFormattersByName_ShouldReturnPageOfFieldFormatters()
        {
            CreateDummyFieldFormatters();

            // Act
            FieldFormatterPagedSearchRequest search = new(1, 5, "DummyFieldFormatter1", null);
            PagedSearchResult<FieldFormatterModel> pagedResult = await fieldFormatterService.SearchFieldFormattersAsync(wipProject.GetId(), search);

            // Assert
            List<FieldFormatterModel> expectedFieldFormatters = [.. wipProject.FieldFormatters.Values
                .Where(clazz=> clazz.Name!.Contains("DummyFieldFormatter1"))
                .Select(clazz => FieldFormatterMapper.ToDto(clazz))];

            pagedResult.ShouldNotBeNull();
            pagedResult.PageNumber.ShouldBe(1);
            pagedResult.PageSize.ShouldBe(5);
            pagedResult.Count.ShouldBe(wipProject.FieldFormatters.Count);
            pagedResult.Result.ShouldBeEquivalentTo(expectedFieldFormatters.Take(5).ToList());
        }

        private void CreateDummyFieldFormatters()
        {
            for (int i = 0; i < 10; i++)
            {
                wipProject.AddFieldFormatter(new EnumFormatter($"DummyFieldFormatter{i + 1}", ["ON", "OFF"]));
            }
        }

        [Fact]
        public async Task GetProjectFieldFormatters_ShouldReturnListOfProjectFieldFormatters()
        {
            // GIVEN
            List<FieldFormatterModel> expectedFieldFormatters = [new EnumFormatterModel(AlarmStatesFormatterName, AlarmStatesFormatterValues)];
            // WHEN
            List<FieldFormatterModel> fieldFormatters = await fieldFormatterService.GetFieldFormattersAsync(runnableProject.GetId());

            // THEN
            fieldFormatters
                .ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedFieldFormatters);
        }

        [Fact]
        public async Task GetProjectFieldFormatter_ShouldReturnFieldFormatter()
        {
            // GIVEN
            FieldFormatterModel expectedFieldFormatter = new EnumFormatterModel(AlarmStatesFormatterName, AlarmStatesFormatterValues);
            // WHEN
            FieldFormatterModel fieldFormatter = await fieldFormatterService.GetFieldFormatterAsync(runnableProject.GetId(), expectedFieldFormatter.Name);

            // THEN
            fieldFormatter
                .ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedFieldFormatter);
        }

        [Fact]
        public async Task CreateProjectFieldFormatter_ShouldCreateFieldFormatter()
        {
            // GIVEN
            FieldFormatterModel expectedFieldFormatter = new EnumFormatterModel($"{AlarmStatesFormatterName}New", AlarmStatesFormatterValues);
            // WHEN
            FieldFormatterModel fieldFormatter = await fieldFormatterService.CreateFieldFormatterAsync(wipProject.GetId(), expectedFieldFormatter);

            // THEN
            fieldFormatter
                .ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedFieldFormatter);
        }

        [Fact]
        public async Task UpdateProjectFieldFormatter_ShouldUpdateFieldFormatter()
        {
            // GIVEN
            FieldFormatterModel expectedFieldFormatter = new EnumFormatterModel(AlarmStatesFormatterName, AlarmStatesFormatterValues);
            // WHEN
            FieldFormatterModel fieldFormatter = await fieldFormatterService.UpdateFieldFormatterAsync(wipProject.GetId(), expectedFieldFormatter.Name, expectedFieldFormatter);

            // THEN
            fieldFormatter
                .ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedFieldFormatter);
        }

        [Fact]
        public async Task DeleteWipProjectFieldFormatter_ShouldDeleteFieldFormatter()
        {
            // GIVEN
            FieldFormatterModel expectedFieldFormatter = new EnumFormatterModel(AlarmStatesFormatterName, AlarmStatesFormatterValues);
            wipProject.GetClass(ClassName).GetField(AlarmStateFieldName).Formatter = null;
            wipProject.GetClass(AllFieldsClassName).GetField("IntFieldWithFormat").Formatter = null;

            // WHEN
            await fieldFormatterService.DeleteFieldFormatterAsync(wipProject.GetId(), expectedFieldFormatter.Name);

            // THEN
            Assert.Throws<UnknownEntityException>(() => wipProject.GetFormatter(expectedFieldFormatter.Name));
        }

        [Fact]
        public async Task DeleteInUseWipProjectFieldFormatter_ThrowsException()
        {
            // GIVEN
            FieldFormatterModel expectedFieldFormatter = new EnumFormatterModel(AlarmStatesFormatterName, AlarmStatesFormatterValues);

            // WHEN/THEN
            await Assert.ThrowsAsync<EntityInUseException>(async () => await fieldFormatterService.DeleteFieldFormatterAsync(wipProject.GetId(), expectedFieldFormatter.Name));
        }

        [Fact]
        public async Task DeleteRunnableProjectFieldFormatter_ShouldThrowException()
        {
            // GIVEN
            FieldFormatterModel expectedFieldFormatter = new EnumFormatterModel($"{AlarmStatesFormatterName}New", AlarmStatesFormatterValues);
            // WHEN
            await Assert.ThrowsAsync<NonWipProjectException>(async () =>
                await fieldFormatterService.DeleteFieldFormatterAsync(runnableProject.GetId(), expectedFieldFormatter.Name));

            // THEN
        }

        [Fact]
        public async Task DeleteNonExistentFieldFormatter_ShouldThrowException()
        {
            // GIVEN
            // WHEN
            await Assert.ThrowsAsync<UnknownEntityException>(async () =>
                await fieldFormatterService.DeleteFieldFormatterAsync(wipProject.GetId(), "UnknownFormatter"));

            // THEN
        }
    }
}
