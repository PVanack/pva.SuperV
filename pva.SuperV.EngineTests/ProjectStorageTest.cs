using pva.Helpers.Extensions;
using pva.SuperV.Engine;
using pva.SuperV.Engine.FieldFormatters;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.Engine.Processing;
using Shouldly;

namespace pva.SuperV.EngineTests
{
    [Collection("Project building")]
    public class ProjectStorageTest : SuperVTestsBase
    {
        [Fact]
        public void GivenProjectWithClassAndField_WhenSavingAndReloadingRunnableProjectDefinition_ThenReloadedProjectDefinitionIsSameAsSavedProject()
        {
            // GIVEN

            // WHEN
            RunnableProject expectedProject = CreateRunnableProject(NullHistoryStorageEngine.Prefix);
            var instance = expectedProject.CreateInstance(ClassName, InstanceName);
            string filename = ProjectStorage.SaveProjectDefinition(expectedProject);
            // Tweak to keep the expected project and avoid it being unloaded!
            WipProject? loadedProject = ProjectStorage.LoadProjectDefinition<WipProject>(filename);

            // THEN
            CheckProjectProperties(expectedProject, loadedProject!);
            CheckProjectClasses(expectedProject, loadedProject!);
            CheckProjectFieldFormatters(expectedProject, loadedProject!);

            instance!.Dispose();
            DeleteProject(expectedProject);
        }

        [Fact]
        public void GivenProjectWithInstances_WhenSavingAndReloadingRunnableProjectSnapshot_ThenReloadedProjectInstancesAreSameAsSavedProject()
        {
            // GIVEN

            // WHEN
            RunnableProject project = CreateRunnableProject(NullHistoryStorageEngine.Prefix);
            Instance? instance = project.CreateInstance(ClassName, InstanceName);
            Field<int>? intField = instance!.GetField<int>(ValueFieldName);
            intField!.SetValue(314);
            string filename = ProjectStorage.SaveProjectInstances(project);
            project.Instances.Clear();

            ProjectStorage.LoadProjectInstances(project, filename);

            // THEN
            project.Instances.Count.ShouldBe(1);
            Instance loadedInstance = project.GetInstance(InstanceName);
            loadedInstance.ShouldNotBeNull();
            loadedInstance.ShouldNotBeSameAs(instance);
            loadedInstance.Name.ShouldBe(instance.Name);
            loadedInstance.Class.Name.ShouldBe(instance.Class.Name);
            loadedInstance.Fields.Count.ShouldBe(7);
            Field<int>? loadedField = loadedInstance.GetField<int>(ValueFieldName);
            loadedField!.Value.ShouldBe(intField.Value);
            loadedField!.Value.ToString().ShouldBe(intField.Value.ToString());
            loadedField!.Instance.ShouldBe(loadedInstance);

            instance.Dispose();
            DeleteProject(project);
        }

        private static void CheckProjectProperties(Project expectedProject, Project loadedProject)
        {
            loadedProject.ShouldNotBeNull();
            loadedProject.Name.ShouldBe(expectedProject.Name);
            loadedProject.Description.ShouldBe(expectedProject.Description);
            loadedProject.Classes.Count.ShouldBe(expectedProject.Classes.Count);
        }

        private static void CheckProjectClasses(Project expectedProject, Project loadedProject)
        {
            expectedProject.Classes
                .ForEach((k, v) =>
                {
                    loadedProject.Classes.ShouldContainKey(k);
                    Class? loadedClass = loadedProject.Classes.GetValueOrDefault(k);
                    loadedClass!.Name.ShouldBe(v.Name);
                    loadedClass!.FieldDefinitions.Count.ShouldBe(v.FieldDefinitions.Count);
                    CheckClassFieldDefinitions(v, loadedClass);
                });
        }

        private static void CheckClassFieldDefinitions(Class expectedClass, Class loadedClass)
        {
            expectedClass.FieldDefinitions.ForEach((k, v) =>
            {
                loadedClass.FieldDefinitions.ShouldContainKey(k);
                IFieldDefinition? loadedFieldDefinition = loadedClass.FieldDefinitions.GetValueOrDefault(k);
                IFieldDefinition? savedFieldDefinition = v;
                loadedFieldDefinition.ShouldNotBeNull();
                loadedFieldDefinition!.Name.ShouldBe(savedFieldDefinition.Name);
                loadedFieldDefinition!.Type.ShouldBe(savedFieldDefinition.Type);
                Assert.Equal(((dynamic)loadedFieldDefinition)!.DefaultValue, ((dynamic)savedFieldDefinition)!.DefaultValue);
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

        private static void CheckProjectFieldFormatters(Project expectedProject, Project loadedProject)
        {
            expectedProject.FieldFormatters
                .ForEach((k, v) =>
                {
                    loadedProject.FieldFormatters.ShouldContainKey(k);
                    FieldFormatter? loadedFieldFormatter = loadedProject.FieldFormatters.GetValueOrDefault(k);
                    loadedFieldFormatter!.ShouldBeEquivalentTo(v);
                });
        }
    }
}