using pva.SuperV.Api.Exceptions;
using pva.SuperV.Api.Services.Classes;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.EngineTests;
using pva.SuperV.Model;
using pva.SuperV.Model.Classes;
using Shouldly;

namespace pva.SuperV.ApiTests
{
    [Collection("Project building")]
    public class ClassServiceTests : SuperVTestsBase
    {
        private readonly ClassService classService;
        private readonly RunnableProject runnableProject;
        private readonly WipProject wipProject;

        public ClassServiceTests()
        {
            classService = new(LoggerFactory);
            runnableProject = CreateRunnableProject();
            wipProject = CreateWipProject(null);
        }

        [Fact]
        public async Task SearchClassesPaged_ShouldReturnPageOfClasses()
        {
            CreateDummyClasses();

            // Act
            ClassPagedSearchRequest search = new(1, 5, null, null);
            PagedSearchResult<ClassModel> page1Result = await classService.SearchClassesAsync(wipProject.GetId(), search);
            search = search with { PageNumber = 2, PageSize = 10 };
            PagedSearchResult<ClassModel> page2Result = await classService.SearchClassesAsync(wipProject.GetId(), search);
            search = search with { PageNumber = 3 };
            PagedSearchResult<ClassModel> page3Result = await classService.SearchClassesAsync(wipProject.GetId(), search);
            search = search with { PageNumber = 4 };
            PagedSearchResult<ClassModel> page4Result = await classService.SearchClassesAsync(wipProject.GetId(), search);

            // Assert
            List<ClassModel> expectedClasses = [.. wipProject.Classes.Select(entry => ClassMapper.ToDto(entry.Value))];

            page1Result.ShouldNotBeNull();
            page1Result.PageNumber.ShouldBe(1);
            page1Result.PageSize.ShouldBe(5);
            page1Result.Count.ShouldBe(wipProject.Classes.Count);
            page1Result.Result.ShouldBeEquivalentTo(expectedClasses.Take(5).ToList());

            page2Result.ShouldNotBeNull();
            page2Result.PageNumber.ShouldBe(2);
            page2Result.PageSize.ShouldBe(10);
            page2Result.Count.ShouldBe(wipProject.Classes.Count);
            page2Result.Result.ShouldBeEquivalentTo(expectedClasses.Skip(10).Take(10).ToList());

            page3Result.ShouldNotBeNull();
            page3Result.PageNumber.ShouldBe(3);
            page3Result.PageSize.ShouldBe(10);
            page3Result.Count.ShouldBe(wipProject.Classes.Count);
            page3Result.Result.ShouldBeEquivalentTo(expectedClasses.Skip(20).Take(10).ToList());

            page4Result.ShouldNotBeNull();
            page4Result.PageNumber.ShouldBe(4);
            page4Result.PageSize.ShouldBe(10);
            page4Result.Count.ShouldBe(wipProject.Classes.Count);
            page4Result.Result.ShouldBeEmpty();
        }

        [Fact]
        public async Task SearchClassesSortedByNameAsc_ShouldReturnPageOfClassesSorted()
        {
            CreateDummyClasses();

            // Act
            ClassPagedSearchRequest search = new(1, 5, null, "name");
            PagedSearchResult<ClassModel> pagedResult = await classService.SearchClassesAsync(wipProject.GetId(), search);

            // Assert
            List<ClassModel> expectedClasses = [.. wipProject.Classes.Select(entry => ClassMapper.ToDto(entry.Value))];
            expectedClasses.Sort(new Comparison<ClassModel>((a, b) => a.Name.CompareTo(b.Name)));

            pagedResult.ShouldNotBeNull();
            pagedResult.PageNumber.ShouldBe(1);
            pagedResult.PageSize.ShouldBe(5);
            pagedResult.Count.ShouldBe(expectedClasses.Count);
            pagedResult.Result.ShouldBeEquivalentTo(expectedClasses.Take(5).ToList());
        }

        [Fact]
        public async Task SearchClassesSortedByNameDesc_ShouldReturnPageOfClassesSorted()
        {
            CreateDummyClasses();

            // Act
            ClassPagedSearchRequest search = new(1, 5, null, "-name");
            PagedSearchResult<ClassModel> pagedResult = await classService.SearchClassesAsync(wipProject.GetId(), search);

            // Assert
            List<ClassModel> expectedClasses = [.. wipProject.Classes.Select(entry => ClassMapper.ToDto(entry.Value))];
            expectedClasses.Sort(new Comparison<ClassModel>((a, b) => a.Name.CompareTo(b.Name)));
            expectedClasses.Reverse();

            pagedResult.ShouldNotBeNull();
            pagedResult.PageNumber.ShouldBe(1);
            pagedResult.PageSize.ShouldBe(5);
            pagedResult.Count.ShouldBe(expectedClasses.Count);
            pagedResult.Result.ShouldBeEquivalentTo(expectedClasses.Take(5).ToList());
        }

        [Fact]
        public async Task SearchClassesSortedWithInvalidOption_ShouldThrowException()
        {
            // Act
            ClassPagedSearchRequest search = new(1, 5, null, "-InvalidOption");
            await Assert.ThrowsAsync<InvalidSortOptionException>(async () => await classService.SearchClassesAsync(wipProject.GetId(), search));
        }

        [Fact]
        public async Task SearchClassesByName_ShouldReturnPageOfClasses()
        {
            CreateDummyClasses();

            // Act
            ClassPagedSearchRequest search = new(1, 5, "DummyClass1", null);
            PagedSearchResult<ClassModel> pagedResult = await classService.SearchClassesAsync(wipProject.GetId(), search);

            // Assert
            List<ClassModel> expectedClasses = [.. wipProject.Classes.Values
                .Where(clazz=> clazz.Name!.Contains("DummyClass1"))
                .Select(clazz => ClassMapper.ToDto(clazz))];

            pagedResult.ShouldNotBeNull();
            pagedResult.PageNumber.ShouldBe(1);
            pagedResult.PageSize.ShouldBe(5);
            pagedResult.Count.ShouldBe(wipProject.Classes.Count);
            pagedResult.Result.ShouldBeEquivalentTo(expectedClasses.Take(5).ToList());
        }


        private void CreateDummyClasses()
        {
            for (int i = 0; i < 10; i++)
            {
                wipProject.AddClass($"DummyClass{i + 1}");
            }
        }

        [Fact]
        public async Task GetClasses_ShouldReturnListOfClasses()
        {
            // Act
            var result = await classService.GetClassesAsync(runnableProject.GetId());

            // Assert
            result.Count.ShouldBe(3);
            result.ShouldContain(c => c.Name == ClassName);
            result.ShouldContain(c => c.Name == BaseClassName);
            result.ShouldContain(c => c.Name == AllFieldsClassName);
        }

        [Fact]
        public async Task GetClass_ShouldReturnClass_WhenClassExists()
        {
            // Act
            var result = await classService.GetClassAsync(runnableProject.GetId(), ClassName);

            // Assert
            result.ShouldNotBeNull();
            result.Name.ShouldBe(ClassName);
        }

        [Fact]
        public async Task GetClass_ShouldThrowUnknownEntityException_WhenClassDoesNotExist()
        {
            // Act & Assert
            await Assert.ThrowsAsync<UnknownEntityException>(async ()
                => await classService.GetClassAsync(runnableProject.GetId(), "UnknownClass"));
        }

        [Fact]
        public async Task CreateClassInWipProject_ShouldCreateClass()
        {
            ClassModel expectedClass = new("NewClass", null);
            // Act & Assert
            ClassModel createClassModel = await classService.CreateClassAsync(wipProject.GetId(), expectedClass);

            createClassModel.ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedClass);
        }

        [Fact]
        public async Task UpdateClassInWipProject_ShouldUpdateClass()
        {
            ClassModel expectedClass = new(ClassName, null);
            // Act & Assert
            ClassModel createClassModel = await classService.UpdateClassAsync(wipProject.GetId(), expectedClass.Name, expectedClass);

            createClassModel.ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedClass);
        }

        [Fact]
        public async Task DeleteClassInWipProject_ShouldDeleteClass()
        {
            ClassModel expectedClass = new("NewClass", null);
            // Act
            await classService.DeleteClassAsync(wipProject.GetId(), expectedClass.Name);

            // Assert
            wipProject.Classes.ShouldNotContainKey(expectedClass.Name);
        }
    }
}
