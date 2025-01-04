using pva.SuperV.Engine.Exceptions;
using System.Text.RegularExpressions;

namespace pva.SuperV.Engine
{
    /// <summary>
    /// SuperV Project class. It contains all the information required (<see cref="Class"/>, <see cref="Instance"/>, processing).
    /// </summary>
    public abstract partial class Project : IDisposable
    {
        /// <summary>
        /// Regex for validating project name.
        /// </summary>
        [GeneratedRegex(Constants.IdentifierNamePattern)]
        private static partial Regex ProjectNameRegex();

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
            get { return _name; }
            set
            {
                ValidateName(value!);
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
        /// Creates an empty <see cref="WipProject"/>.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <returns>The created <see cref="WipProject"/></returns>
        public static WipProject CreateProject(string projectName)
        {
            WipProject project = new(projectName);
            return project;
        }

        /// <summary>
        /// Creates a <see cref="WipProject"/> from a <see cref="RunnableProject"/> for modification.
        /// </summary>
        /// <param name="runnableProject">The runnable project from which to create the new <see cref="WipProject"/>.</param>
        /// <returns>The new <see cref="WipProject"/></returns>
        public static WipProject CreateProject(RunnableProject runnableProject)
        {
            return new WipProject(runnableProject);
        }

        /// <summary>
        /// Validates the name of the project.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="pva.SuperV.Engine.Exceptions.InvalidProjectNameException"></exception>
        private static void ValidateName(string name)
        {
            if (!ProjectNameRegex().IsMatch(name))
            {
                throw new InvalidProjectNameException(name, Constants.IdentifierNamePattern);
            }
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

            throw new UnknownClassException(className);
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

            throw new UnknownFormatterException(formatterName);
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
        public virtual void Unload()
        {
            Classes.Clear();
#pragma warning disable S1215 // "GC.Collect" should not be called
            GC.Collect();
#pragma warning restore S1215 // "GC.Collect" should not be called
            GC.WaitForPendingFinalizers();
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
    }
}