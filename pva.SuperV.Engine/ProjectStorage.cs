﻿using pva.Helpers.Extensions;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.Engine.JsonConverters;
using System.Text.Json;

namespace pva.SuperV.Engine
{
    /// <summary>
    /// Class for saving/loading <see cref="Project"/> definitions and snapshot files.
    /// </summary>
    public static class ProjectStorage
    {
        /// <summary>
        /// Saves a project definition.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="project">The project for which definitions should be saved.</param>
        /// <returns>Name of saved file.</returns>
        public static string SaveProjectDefinition<T>(T project) where T : Project
        {
            string filename = Path.Combine(Project.ProjectsPath, $"{project.Name}.{project.Version}.prj");
            SaveProjectDefinition(project, filename);
            return filename;
        }

        /// <summary>
        /// Saves a project definition.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="project">The project.</param>
        /// <param name="filename">The filename.</param>
        public static void SaveProjectDefinition<T>(T project, string filename) where T : Project
        {
            using StreamWriter outputFile = new(filename);
            outputFile.WriteLine(JsonSerializer.Serialize(project));
        }

        /// <summary>
        /// Loads a project definition from a file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public static T LoadProjectDefinition<T>(string filename) where T : Project
        {
            string json = File.ReadAllText(filename);
            T? projectInstance = JsonSerializer.Deserialize<T>(json);
            projectInstance!.HistoryStorageEngine = HistoryStorageEngineFactory.CreateHistoryStorageEngine(projectInstance.HistoryStorageEngineConnectionString);
            projectInstance.Classes.Values.ForEach(clazz =>
            {
                if (clazz.BaseClassName != null)
                {
                    clazz.BaseClass = projectInstance.GetClass(clazz.BaseClassName);
                }
                clazz.FieldDefinitions.Values.ForEach(field =>
                {
                    field.ValuePostChangeProcessings.ForEach(postProcessing =>
                        postProcessing.BuildAfterDeserialization(projectInstance, clazz));
                });
            });
            return projectInstance;
        }

        /// <summary>
        /// Saves a project instances.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>File name where project instances were saved.</returns>
        public static string SaveProjectInstances(RunnableProject project)
        {
            string filename = Path.Combine(Project.ProjectsPath, $"{project.Name}.{project.Version}.snp");
            SaveProjectInstances(project, filename);
            return filename;
        }

        /// <summary>
        /// Saves a project instances into a file.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="filename">The filename.</param>
        public static void SaveProjectInstances(RunnableProject project, string filename)
        {
            using StreamWriter outputFile = new(filename);
            Dictionary<string, IInstance> instances = new(project.Instances.Count);
            project.Instances.ForEach((k, v) =>
            {
                var instance = v as IInstance;
                instances.Add(k, instance!);
            });
            outputFile.WriteLine(JsonSerializer.Serialize(instances));
        }

        /// <summary>
        /// Loads the project instances from a file.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="filename">The filename.</param>
        public static void LoadProjectInstances(RunnableProject project, string filename)
        {
            InstanceJsonConverter.LoadedProject = project;
            // Instances are already added to project as deserialization uses project.CreateInstance()
            JsonSerializer.Deserialize<Dictionary<string, IInstance>>(File.ReadAllText(filename));
        }
    }
}