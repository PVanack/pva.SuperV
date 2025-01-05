using FluentAssertions;
using pva.Helpers.Extensions;
using pva.SuperV.Engine;

namespace pva.SuperV.EngineTests
{
    [Collection("Project building")]
    public class ProjectStorageTest
    {
        [Fact]
        public void GivenProjectWithClassAndField_WhenSavingAndReloadingRunnableProjectDefinition_ThenReloadedProjectDefinitionIsSameAsSavedProject()
        {
            // GIVEN

            // WHEN
            RunnableProject project = ProjectHelpers.CreateRunnableProject();
            var instance = project.CreateInstance(ProjectHelpers.ClassName, ProjectHelpers.InstanceName);
            string filename = ProjectStorage.SaveProjectDefinition(project);

            RunnableProject? loadedProject = ProjectStorage.LoadProjectDefinition<RunnableProject>(filename);

            // THEN
            CheckProjectProperties(project, loadedProject!);
            CheckProjectClasses(project, loadedProject!);
            CheckProjectFieldFormatters(project, loadedProject!);

            instance!.Dispose();
            ProjectHelpers.DeleteProject(project);
        }

        [Fact]
        public void GivenProjectWithInstances_WhenSavingAndReloadingRunnableProjectSnapshot_ThenReloadedProjectInstancesAreSameAsSavedProject()
        {
            // GIVEN

            // WHEN
            RunnableProject project = ProjectHelpers.CreateRunnableProject();
            var instance = project.CreateInstance(ProjectHelpers.ClassName, ProjectHelpers.InstanceName);
            Field<int> intField = instance!.GetField<int>(ProjectHelpers.ValueFieldName);
            intField.Value = 314;
            string filename = ProjectStorage.SaveProjectInstances(project);
            project.Instances.Clear();

            ProjectStorage.LoadProjectInstances(project, filename);

            // THEN
            project.Instances.Count.Should().Be(1);
            var loadedInstance = project.GetInstance(ProjectHelpers.InstanceName);
            loadedInstance.Should().NotBeSameAs(instance);
            loadedInstance.Name.Should().Be(instance.Name);
            loadedInstance.Class.Name.Should().Be(instance.Class.Name);
            loadedInstance.Fields.Count.Should().Be(6);
            Field<int>? loadedField = loadedInstance.GetField<int>(ProjectHelpers.ValueFieldName);
            loadedField!.Value.Should().Be(intField.Value);
            loadedField!.Value.ToString().Should().Be(intField.Value.ToString());
            loadedField!.Instance.Should().Be(loadedInstance);

            instance.Dispose();
            ProjectHelpers.DeleteProject(project);
        }

        private static void CheckProjectProperties(RunnableProject project, RunnableProject loadedProject)
        {
            loadedProject.Should().NotBeNull();
            loadedProject.Name.Should().Be(project.Name);
            loadedProject.Description.Should().Be(project.Description);
            loadedProject.Classes.Count.Should().Be(project.Classes.Count);
        }

        private static void CheckProjectClasses(RunnableProject project, RunnableProject loadedProject)
        {
            project.Classes
                .ForEach((k, v) =>
                {
                    loadedProject.Classes.Should().ContainKey(k);
                    Class? loadedClass = loadedProject.Classes.GetValueOrDefault(k);
                    loadedClass!.Name.Should().Be(v.Name);
                    loadedClass!.FieldDefinitions.Count.Should().Be(v.FieldDefinitions.Count);
                    CheckClassFieldDefinitions(v, loadedClass);
                });
        }

        private static void CheckClassFieldDefinitions(Class savedClass, Class loadedClass)
        {
            savedClass.FieldDefinitions.ForEach((k, v) =>
            {
                loadedClass.FieldDefinitions.Should().ContainKey(k);
                IFieldDefinition? loadedFieldDefinition = loadedClass.FieldDefinitions.GetValueOrDefault(k);
                IFieldDefinition? savedFieldDefinition = v;
                loadedFieldDefinition.Name.Should().Be(savedFieldDefinition.Name);
                loadedFieldDefinition.Type.Should().Be(savedFieldDefinition.Type);
                Assert.True(((dynamic)loadedFieldDefinition)!.DefaultValue.Equals(((dynamic)savedFieldDefinition)!.DefaultValue));
                FieldFormatter? loadedFieldFormatter = loadedFieldDefinition!.Formatter;
                FieldFormatter? savedFieldFormatter = savedFieldDefinition!.Formatter;
                loadedFieldFormatter?.Should().BeEquivalentTo(savedFieldFormatter);
                loadedFieldDefinition?.ValuePostChangeProcessings.Should().HaveCount(savedFieldDefinition.ValuePostChangeProcessings.Count);
                for (int index = 0; index < savedFieldDefinition?.ValuePostChangeProcessings.Count; index++)
                {
                    IFieldValueProcessing savedProcessing = savedFieldDefinition.ValuePostChangeProcessings[index];
                    IFieldValueProcessing loadedProcesing = loadedFieldDefinition!.ValuePostChangeProcessings[index];
                    loadedProcesing?.Should().BeEquivalentTo(savedProcessing);
                }
            });
        }

        private static void CheckProjectFieldFormatters(RunnableProject project, RunnableProject loadedProject)
        {
            project.FieldFormatters
                .ForEach((k, v) =>
                {
                    loadedProject.FieldFormatters.Should().ContainKey(k);
                    FieldFormatter? loadedFieldFormatter = loadedProject.FieldFormatters.GetValueOrDefault(k);
                    loadedFieldFormatter!.Should().BeEquivalentTo(v);
                });
        }
    }
}