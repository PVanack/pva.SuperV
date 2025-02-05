using pva.Helpers.Extensions;
using pva.SuperV.Engine;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.Engine.Processing;
using Shouldly;

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
            RunnableProject project = ProjectHelpers.CreateRunnableProject(NullHistoryStorageEngine.Prefix);
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
            RunnableProject project = ProjectHelpers.CreateRunnableProject(NullHistoryStorageEngine.Prefix);
            Instance? instance = project.CreateInstance(ProjectHelpers.ClassName, ProjectHelpers.InstanceName) as Instance;
            Field<int>? intField = instance!.GetField<int>(ProjectHelpers.ValueFieldName);
            intField!.SetValue(314);
            string filename = ProjectStorage.SaveProjectInstances(project);
            project.Instances.Clear();

            ProjectStorage.LoadProjectInstances(project, filename);

            // THEN
            project.Instances.Count.ShouldBe(1);
            Instance loadedInstance = project.GetInstance(ProjectHelpers.InstanceName);
            loadedInstance.ShouldNotBeNull();
            loadedInstance.ShouldNotBeSameAs(instance);
            loadedInstance.Name.ShouldBe(instance.Name);
            loadedInstance.Class.Name.ShouldBe(instance.Class.Name);
            loadedInstance.Fields.Count.ShouldBe(7);
            Field<int>? loadedField = loadedInstance.GetField<int>(ProjectHelpers.ValueFieldName);
            loadedField!.Value.ShouldBe(intField.Value);
            loadedField!.Value.ToString().ShouldBe(intField.Value.ToString());
            loadedField!.Instance.ShouldBe(loadedInstance);

            instance.Dispose();
            ProjectHelpers.DeleteProject(project);
        }

        private static void CheckProjectProperties(RunnableProject project, RunnableProject loadedProject)
        {
            loadedProject.ShouldNotBeNull();
            loadedProject.Name.ShouldBe(project.Name);
            loadedProject.Description.ShouldBe(project.Description);
            loadedProject.Classes.Count.ShouldBe(project.Classes.Count);
        }

        private static void CheckProjectClasses(RunnableProject project, RunnableProject loadedProject)
        {
            project.Classes
                .ForEach((k, v) =>
                {
                    loadedProject.Classes.ShouldContainKey(k);
                    Class? loadedClass = loadedProject.Classes.GetValueOrDefault(k);
                    loadedClass!.Name.ShouldBe(v.Name);
                    loadedClass!.FieldDefinitions.Count.ShouldBe(v.FieldDefinitions.Count);
                    CheckClassFieldDefinitions(v, loadedClass);
                });
        }

        private static void CheckClassFieldDefinitions(Class savedClass, Class loadedClass)
        {
            savedClass.FieldDefinitions.ForEach((k, v) =>
            {
                loadedClass.FieldDefinitions.ShouldContainKey(k);
                IFieldDefinition? loadedFieldDefinition = loadedClass.FieldDefinitions.GetValueOrDefault(k);
                IFieldDefinition? savedFieldDefinition = v;
                loadedFieldDefinition.ShouldNotBeNull();
                loadedFieldDefinition!.Name.ShouldBe(savedFieldDefinition.Name);
                loadedFieldDefinition!.Type.ShouldBe(savedFieldDefinition.Type);
                Assert.True(((dynamic)loadedFieldDefinition)!.DefaultValue.Equals(((dynamic)savedFieldDefinition)!.DefaultValue));
                FieldFormatter? loadedFieldFormatter = loadedFieldDefinition!.Formatter;
                FieldFormatter? savedFieldFormatter = savedFieldDefinition!.Formatter;
                loadedFieldFormatter?.ShouldBeEquivalentTo(savedFieldFormatter);
                loadedFieldDefinition?.ValuePostChangeProcessings.Count.ShouldBe(savedFieldDefinition.ValuePostChangeProcessings.Count);
                for (int index = 0; index < savedFieldDefinition?.ValuePostChangeProcessings.Count; index++)
                {
                    IFieldValueProcessing savedProcessing = savedFieldDefinition.ValuePostChangeProcessings[index];
                    IFieldValueProcessing loadedProcesing = loadedFieldDefinition!.ValuePostChangeProcessings[index];
                    loadedProcesing?.ShouldBeEquivalentTo(savedProcessing);
                }
            });
        }

        private static void CheckProjectFieldFormatters(RunnableProject project, RunnableProject loadedProject)
        {
            project.FieldFormatters
                .ForEach((k, v) =>
                {
                    loadedProject.FieldFormatters.ShouldContainKey(k);
                    FieldFormatter? loadedFieldFormatter = loadedProject.FieldFormatters.GetValueOrDefault(k);
                    loadedFieldFormatter!.ShouldBeEquivalentTo(v);
                });
        }
    }
}