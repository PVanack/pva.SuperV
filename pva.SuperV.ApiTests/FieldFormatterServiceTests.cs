using pva.SuperV.Api;
using pva.SuperV.Api.Services.FieldFormatters;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.FieldValueFormatters;
using pva.SuperV.EngineTests;
using pva.SuperV.Model.FieldFormatters;
using Shouldly;

namespace pva.SuperV.ApiTests
{
    [Collection("Project building")]
    public class FieldFormatterServiceTests
    {
        private readonly FieldFormatterService _fieldFormatterService;
        private readonly RunnableProject runnableProject;
        private readonly WipProject wipProject;

        public FieldFormatterServiceTests()
        {
            _fieldFormatterService = new();
            runnableProject = ProjectHelpers.CreateRunnableProject();
            wipProject = Project.CreateProject(runnableProject);
        }

        [Fact]
        public void GetFieldFormatterTypes_ShouldReturnListOfFieldFormatterTypes()
        {
            // GIVEN
            // WHEN
            List<string> formatterTypes = _fieldFormatterService.GetFieldFormatterTypes();

            // THEN
            formatterTypes
                .ShouldNotBeNull()
                .ShouldContain(typeof(EnumFormatter).ToString());
        }

        [Fact]
        public void GetProjectFieldFormatters_ShouldReturnListOfProjectFieldFormatters()
        {
            // GIVEN
            List<FieldFormatterModel> expectedFieldFormatters = [new EnumFormatterModel(ProjectHelpers.AlarmStatesFormatterName, ProjectHelpers.AlarmStatesFormatterValues)];
            // WHEN
            List<FieldFormatterModel> fieldFormatters = _fieldFormatterService.GetFieldFormatters(runnableProject.GetId());

            // THEN
            fieldFormatters
                .ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedFieldFormatters);
        }

        [Fact]
        public void GetProjectFieldFormatter_ShouldReturnFieldFormatter()
        {
            // GIVEN
            FieldFormatterModel expectedFieldFormatter = new EnumFormatterModel(ProjectHelpers.AlarmStatesFormatterName, ProjectHelpers.AlarmStatesFormatterValues);
            // WHEN
            FieldFormatterModel fieldFormatter = _fieldFormatterService.GetFieldFormatter(runnableProject.GetId(), expectedFieldFormatter.Name);

            // THEN
            fieldFormatter
                .ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedFieldFormatter);
        }

        [Fact]
        public void CreateProjectFieldFormatter_ShouldCreateFieldFormatter()
        {
            // GIVEN
            FieldFormatterModel expectedFieldFormatter = new EnumFormatterModel($"{ProjectHelpers.AlarmStatesFormatterName}New", ProjectHelpers.AlarmStatesFormatterValues);
            // WHEN
            FieldFormatterModel fieldFormatter = _fieldFormatterService.CreateFieldFormatter(wipProject.GetId(), expectedFieldFormatter);

            // THEN
            fieldFormatter
                .ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedFieldFormatter);
        }

        [Fact]
        public void DeleteWipProjectFieldFormatter_ShouldDeleteFieldFormatter()
        {
            // GIVEN
            FieldFormatterModel expectedFieldFormatter = new EnumFormatterModel($"{ProjectHelpers.AlarmStatesFormatterName}", ProjectHelpers.AlarmStatesFormatterValues);
            // WHEN
            _fieldFormatterService.DeleteFieldFormatter(wipProject.GetId(), expectedFieldFormatter.Name);

            // THEN
            Assert.Throws<UnknownEntityException>(() => wipProject.GetFormatter(expectedFieldFormatter.Name));
        }

        [Fact]
        public void DeleteRunnableProjectFieldFormatter_ShouldThrowException()
        {
            // GIVEN
            FieldFormatterModel expectedFieldFormatter = new EnumFormatterModel($"{ProjectHelpers.AlarmStatesFormatterName}New", ProjectHelpers.AlarmStatesFormatterValues);
            // WHEN
            Assert.Throws<NonWipProjectException>(() =>
                _fieldFormatterService.DeleteFieldFormatter(runnableProject.GetId(), expectedFieldFormatter.Name));

            // THEN
        }

        [Fact]
        public void DeleteNonExistentFieldFormatter_ShouldThrowException()
        {
            // GIVEN
            // WHEN
            Assert.Throws<UnknownEntityException>(() =>
                _fieldFormatterService.DeleteFieldFormatter(wipProject.GetId(), "UnknownFormatter"));

            // THEN
        }
    }
}
