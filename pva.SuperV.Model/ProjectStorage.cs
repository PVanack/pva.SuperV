using pva.Helpers;
using pva.Helpers.Extensions;
using System.Text.Json;

namespace pva.SuperV.Model
{
    public static class ProjectStorage
    {
        public static string SaveProjectDefinition<T>(T project) where T : Project
        {
            string filename = Path.Combine(Path.GetTempPath(), "pva.SuperV", $"{project.Name}.{project.Version}.prj");
            SaveProjectDefinition(project, filename);
            return filename;
        }

        public static void SaveProjectDefinition<T>(T project, string filename) where T : Project
        {
            using StreamWriter outputFile = new(filename);
            outputFile.WriteLine(JsonSerializer.Serialize(project));
        }

        public static T? LoadProjectDefinition<T>(string filename) where T : Project
        {
            string json = File.ReadAllText(filename);
            return JsonSerializer.Deserialize<T>(json);
        }

        public static string SaveProjectInstances(RunnableProject project)
        {
            string filename = Path.Combine(Path.GetTempPath(), "pva.SuperV", $"{project.Name}.{project.Version}.snp");
            SaveProjectInstances(project, filename);
            return filename;
        }

        public static void SaveProjectInstances(RunnableProject project, string filename)
        {
            using StreamWriter outputFile = new(filename);
            Dictionary<string, IInstance> instances = new(project.Instances.Count);
            project.Instances.ForEach((k, v) =>
            {
                instances.Add(k, v as IInstance);
            });
            outputFile.WriteLine(JsonSerializer.Serialize(instances));
        }

        public static void LoadProjectInstances(RunnableProject project, string filename)
        {
            InstanceJsonConverter.LoadedProject = project;
            // Instances are already added to project as deserialization uses project.CreateInstance()
            JsonSerializer.Deserialize<Dictionary<string, IInstance>>(File.ReadAllText(filename));
        }
    }
}
