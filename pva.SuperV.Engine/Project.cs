using pva.Helpers.Extensions;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.HistoryStorage;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json.Serialization;

namespace pva.SuperV.Engine
{
    /// <summary>
    /// SuperV Project class. It contains all the information required (<see cref="Class"/>, <see cref="Instance"/>, processing).
    /// </summary>
    public abstract class Project : IDisposable
    {
        /// <summary>
        /// Gets the projects path where all SuperV stuff is stored (generated assemblies, project definitions and snapshots).
        /// </summary>
        /// <value>
        /// The projects path.
        /// </value>
        public static string ProjectsPath { get; } = Path.Combine(Path.GetTempPath(), "pva.SuperV");

        /// <summary>
        /// Gets or sets the current project.
        /// </summary>
        /// <value>
        /// The current project.
        /// </value>
        public static Project? CurrentProject { get; set; }

        /// <summary>
        /// The name of project. Use <see cref="Name"/> to access its content.
        /// </summary>
        private string? _name;

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        /// <value>
        /// The project name.
        /// </value>
        public string? Name
        {
            get => _name;
            set
            {
                IdentifierValidation.ValidateIdentifier("project", value);
                _name = value;
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public int Version { get; set; }

        /// <summary>
        /// Gets the classes of project.
        /// </summary>
        /// <value>
        /// The classes.
        /// </value>
        public Dictionary<string, Class> Classes { get; init; } = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the field formatters.
        /// </summary>
        /// <value>
        /// The field formatters.
        /// </value>
        public Dictionary<string, FieldFormatter> FieldFormatters { get; init; } = new(StringComparer.OrdinalIgnoreCase);

        private IHistoryStorageEngine? _historyStorageEngine;

        /// <summary>
        /// The history storage engin connection string.
        /// </summary>
        public string? HistoryStorageEngineConnectionString { get; set; }

        /// <summary>
        /// Gets the history repositories.
        /// </summary>
        /// <value>
        /// The history repositories.
        /// </value>
        [JsonIgnore]
        public IHistoryStorageEngine? HistoryStorageEngine
        {
            get => _historyStorageEngine;
            set
            {
                _historyStorageEngine = value;
                HistoryRepositories.Values.ForEach(historyRepository => historyRepository.HistoryStorageEngine = _historyStorageEngine);
            }
        }

        /// <summary>
        /// Gets the history repositories.
        /// </summary>
        /// <value>
        /// The history repositories.
        /// </value>
        public Dictionary<string, HistoryRepository> HistoryRepositories { get; init; } = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// List of projects in use.
        /// </summary>
        public static ConcurrentDictionary<string, Project> Projects { get; } = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Creates an empty <see cref="WipProject"/>.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <returns>The created <see cref="WipProject"/></returns>
        public static WipProject CreateProject(string projectName)
        {
            return CreateProject(projectName, null);
        }

        /// <summary>
        /// Creates an empty <see cref="WipProject"/>.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="historyStorageEngineConnectionString">History storage connection string.</param>
        /// <returns>The created <see cref="WipProject"/></returns>
        public static WipProject CreateProject(string projectName, string? historyStorageEngineConnectionString)
        {
            WipProject project = new(projectName);
            AddProjectToCollection(project);
            if (string.IsNullOrEmpty(historyStorageEngineConnectionString))
            {
                return project;
            }

            project.HistoryStorageEngineConnectionString = historyStorageEngineConnectionString;
            project.HistoryStorageEngine = HistoryStorageEngineFactory.CreateHistoryStorageEngine(historyStorageEngineConnectionString);
            return project;
        }

        private static void AddProjectToCollection(Project project)
        {
            if (Projects.TryGetValue(project.GetId(), out Project previousProject))
            {
                previousProject.Unload();
            }
            Projects[project.GetId()] = project;
        }

        /// <summary>
        /// Creates a <see cref="WipProject"/> from a <see cref="RunnableProject"/> for modification.
        /// </summary>
        /// <param name="runnableProject">The runnable project from which to create the new <see cref="WipProject"/>.</param>
        /// <returns>The new <see cref="WipProject"/></returns>
        public static WipProject CreateProject(RunnableProject runnableProject)
        {
            WipProject wipProject = new(runnableProject);
            AddProjectToCollection(wipProject);
            return wipProject;
        }

        /// <summary>
        /// Builds the specified <see cref="WipProject"/>.
        /// </summary>
        /// <param name="wipProject">The WIP project.</param>
        /// <returns>a <see cref="RunnableProject"/></returns>
        public static RunnableProject Build(WipProject wipProject)
        {
            RunnableProject runnableProject = ProjectBuilder.Build(wipProject);
            AddProjectToCollection(runnableProject);
            return runnableProject;
        }

        /// <summary>
        /// Gets a class for a name.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <returns>Found class</returns>
        /// <exception cref="pva.SuperV.Engine.Exceptions.UnknownClassException"></exception>
        public Class GetClass(string className)
        {
            if (Classes.TryGetValue(className, out Class? value))
            {
                return value;
            }

            throw new UnknownEntityException("Class", className);
        }

        /// <summary>
        /// Finds a class for a name.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <returns><see cref="Class"/> if found or null otherwise.</returns>
        public Class? FindClass(string className)
        {
            try
            {
                return GetClass(className);
            }
            catch { return null; }
        }

        /// <summary>
        /// Gets a formatter for name.
        /// </summary>
        /// <param name="formatterName">Name of the formatter.</param>
        /// <returns><see cref="FieldFormatter"/></returns>
        /// <exception cref="pva.SuperV.Engine.Exceptions.UnknownFormatterException"></exception>
        public FieldFormatter GetFormatter(string formatterName)
        {
            if (FieldFormatters.TryGetValue(formatterName, out FieldFormatter? value))
            {
                return value;
            }

            throw new UnknownEntityException("Field formatter", formatterName);
        }

        /// <summary>
        /// Finds a formatter for name.
        /// </summary>
        /// <param name="formatterName">Name of the formatter.</param>
        /// <returns><see cref="FieldFormatter"/> if found or null otherwise.</returns>
        public FieldFormatter? FindFormatter(string formatterName)
        {
            try
            {
                return GetFormatter(formatterName);
            }
            catch { return null; }
        }

        /// <summary>
        /// Gets the name of the generated assembly file for project.
        /// </summary>
        /// <returns>Assembly file name</returns>
        public string GetAssemblyFileName()
        {
            if (!Directory.Exists(ProjectsPath))
            {
                Directory.CreateDirectory(ProjectsPath);
            }
            return Path.Combine(ProjectsPath, $"{Name}-V{Version}.dll");
        }

        /// <summary>
        /// Gets the project highest version from generated assemblies of project.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <returns>The highest version or 0 if first version.</returns>
        protected static int GetProjectHighestVersion(string projectName)
        {
            return Directory.Exists(ProjectsPath)
                ? Directory.EnumerateFiles(ProjectsPath, $"{projectName}-V*.dll")
                    .Select(fileName =>
                    Convert.ToInt32(fileName
                        .Replace(ProjectsPath, "")
                        .Replace(Path.DirectorySeparatorChar.ToString(), "")
                        .Replace($"{projectName}-V", "")
                        .Replace(".dll", "")))
                    .Order()
                    .LastOrDefault()
                : 0;
        }

        /// <summary>
        /// Gets the next version for project.
        /// </summary>
        /// <returns>Next project version</returns>
        protected int GetNextVersion()
        {
            return GetProjectHighestVersion(Name!) + 1;
        }

        /// <summary>
        /// Unloads the project.
        /// </summary>
        public static void Unload(Project project)
        {
            WeakReference? projectAssemblyLoaderWeakRef = null;
            if (project is RunnableProject runnableProject)
            {
                projectAssemblyLoaderWeakRef = runnableProject.ProjectAssemblyLoaderWeakRef;
            }
            project.Dispose();
            project = null;
            if (projectAssemblyLoaderWeakRef is not null)
            {
                CallGcCleanup(projectAssemblyLoaderWeakRef);
            }
        }


        /// <summary>
        /// Unloads the project.
        /// </summary>
        public virtual void Unload()
        {
            FieldFormatters.Clear();
            Classes.Clear();
            Projects.Remove(GetId(), out _);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S1215:\"GC.Collect\" should not be called",
            Justification = "Need to call GC to remove references to assembly")]
        public static void CallGcCleanup(WeakReference palWeakRef)
        {
            for (int i = 0; palWeakRef.IsAlive && i < 10; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            Unload();
        }

        public abstract string GetId();
    }
}