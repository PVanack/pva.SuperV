﻿using pva.SuperV.Api.Exceptions;
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
        public void GetFieldFormatterTypes_ShouldReturnListOfFieldFormatterTypes()
        {
            // GIVEN
            // WHEN
            List<string> formatterTypes = fieldFormatterService.GetFieldFormatterTypes();

            // THEN
            formatterTypes
                .ShouldNotBeNull()
                .ShouldContain(typeof(EnumFormatter).ToString());
        }

        [Fact]
        public void SearchClassesPaged_ShouldReturnPageOfClasses()
        {
            CreateDummyFieldFormatters();

            // Act
            FieldFormatterPagedSearchRequest search = new(1, 5, null, null);
            PagedSearchResult<FieldFormatterModel> page1Result = fieldFormatterService.SearchFieldFormatters(wipProject.GetId(), search);
            search = search with { PageNumber = 2, PageSize = 10 };
            PagedSearchResult<FieldFormatterModel> page2Result = fieldFormatterService.SearchFieldFormatters(wipProject.GetId(), search);
            search = search with { PageNumber = 3 };
            PagedSearchResult<FieldFormatterModel> page3Result = fieldFormatterService.SearchFieldFormatters(wipProject.GetId(), search);
            search = search with { PageNumber = 4 };
            PagedSearchResult<FieldFormatterModel> page4Result = fieldFormatterService.SearchFieldFormatters(wipProject.GetId(), search);

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
        public void SearchClassesSortedByNameAsc_ShouldReturnPageOfFieldFormattersSorted()
        {
            CreateDummyFieldFormatters();

            // Act
            FieldFormatterPagedSearchRequest search = new(1, 5, null, "name");
            PagedSearchResult<FieldFormatterModel> pagedResult = fieldFormatterService.SearchFieldFormatters(wipProject.GetId(), search);

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
        public void SearchFieldFormattersSortedByNameDesc_ShouldReturnPageOfFieldFormattersSorted()
        {
            CreateDummyFieldFormatters();

            // Act
            FieldFormatterPagedSearchRequest search = new(1, 5, null, "-name");
            PagedSearchResult<FieldFormatterModel> pagedResult = fieldFormatterService.SearchFieldFormatters(wipProject.GetId(), search);

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
        public void SearchFieldFormattersSortedWithInvalidOption_ShouldThrowException()
        {
            // Act
            FieldFormatterPagedSearchRequest search = new(1, 5, null, "-InvalidOption");
            Assert.Throws<InvalidSortOptionException>(() => fieldFormatterService.SearchFieldFormatters(wipProject.GetId(), search));
        }

        [Fact]
        public void SearchFieldFormattersByName_ShouldReturnPageOfFieldFormatters()
        {
            CreateDummyFieldFormatters();

            // Act
            FieldFormatterPagedSearchRequest search = new(1, 5, "DummyFieldFormatter1", null);
            PagedSearchResult<FieldFormatterModel> pagedResult = fieldFormatterService.SearchFieldFormatters(wipProject.GetId(), search);

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
        public void GetProjectFieldFormatters_ShouldReturnListOfProjectFieldFormatters()
        {
            // GIVEN
            List<FieldFormatterModel> expectedFieldFormatters = [new EnumFormatterModel(AlarmStatesFormatterName, AlarmStatesFormatterValues)];
            // WHEN
            List<FieldFormatterModel> fieldFormatters = fieldFormatterService.GetFieldFormatters(runnableProject.GetId());

            // THEN
            fieldFormatters
                .ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedFieldFormatters);
        }

        [Fact]
        public void GetProjectFieldFormatter_ShouldReturnFieldFormatter()
        {
            // GIVEN
            FieldFormatterModel expectedFieldFormatter = new EnumFormatterModel(AlarmStatesFormatterName, AlarmStatesFormatterValues);
            // WHEN
            FieldFormatterModel fieldFormatter = fieldFormatterService.GetFieldFormatter(runnableProject.GetId(), expectedFieldFormatter.Name);

            // THEN
            fieldFormatter
                .ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedFieldFormatter);
        }

        [Fact]
        public void CreateProjectFieldFormatter_ShouldCreateFieldFormatter()
        {
            // GIVEN
            FieldFormatterModel expectedFieldFormatter = new EnumFormatterModel($"{AlarmStatesFormatterName}New", AlarmStatesFormatterValues);
            // WHEN
            FieldFormatterModel fieldFormatter = fieldFormatterService.CreateFieldFormatter(wipProject.GetId(), expectedFieldFormatter);

            // THEN
            fieldFormatter
                .ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedFieldFormatter);
        }

        [Fact]
        public void UpdateProjectFieldFormatter_ShouldUpdateFieldFormatter()
        {
            // GIVEN
            FieldFormatterModel expectedFieldFormatter = new EnumFormatterModel(AlarmStatesFormatterName, AlarmStatesFormatterValues);
            // WHEN
            FieldFormatterModel fieldFormatter = fieldFormatterService.UpdateFieldFormatter(wipProject.GetId(), expectedFieldFormatter.Name, expectedFieldFormatter);

            // THEN
            fieldFormatter
                .ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedFieldFormatter);
        }

        [Fact]
        public void DeleteWipProjectFieldFormatter_ShouldDeleteFieldFormatter()
        {
            // GIVEN
            FieldFormatterModel expectedFieldFormatter = new EnumFormatterModel(AlarmStatesFormatterName, AlarmStatesFormatterValues);
            wipProject.GetClass(ClassName).GetField(AlarmStateFieldName).Formatter = null;
            wipProject.GetClass(AllFieldsClassName).GetField("IntFieldWithFormat").Formatter = null;

            // WHEN
            fieldFormatterService.DeleteFieldFormatter(wipProject.GetId(), expectedFieldFormatter.Name);

            // THEN
            Assert.Throws<UnknownEntityException>(() => wipProject.GetFormatter(expectedFieldFormatter.Name));
        }

        [Fact]
        public void DeleteInUseWipProjectFieldFormatter_ThrowsException()
        {
            // GIVEN
            FieldFormatterModel expectedFieldFormatter = new EnumFormatterModel(AlarmStatesFormatterName, AlarmStatesFormatterValues);

            // WHEN/THEN
            Assert.Throws<EntityInUseException>(() => fieldFormatterService.DeleteFieldFormatter(wipProject.GetId(), expectedFieldFormatter.Name));
        }

        [Fact]
        public void DeleteRunnableProjectFieldFormatter_ShouldThrowException()
        {
            // GIVEN
            FieldFormatterModel expectedFieldFormatter = new EnumFormatterModel($"{AlarmStatesFormatterName}New", AlarmStatesFormatterValues);
            // WHEN
            Assert.Throws<NonWipProjectException>(() =>
                fieldFormatterService.DeleteFieldFormatter(runnableProject.GetId(), expectedFieldFormatter.Name));

            // THEN
        }

        [Fact]
        public void DeleteNonExistentFieldFormatter_ShouldThrowException()
        {
            // GIVEN
            // WHEN
            Assert.Throws<UnknownEntityException>(() =>
                fieldFormatterService.DeleteFieldFormatter(wipProject.GetId(), "UnknownFormatter"));

            // THEN
        }
    }
}
