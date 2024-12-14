using pva.SuperV.Model.Exceptions;
using System.Text.RegularExpressions;

namespace pva.SuperV.Model
{
    /// <summary>
    /// SuperV Project class. It contains all the information required (classes, objects, processing).
    /// </summary>
    public abstract partial class Project: IDisposable
    {
        private const string ProjectNamePattern = "^([A-Z]|[a-z]|[0-9])*$";
        private const string ProjectFileNamePattern = @"^(?<folder>.*[\\\/])?(?<filename>\.*.*?)(?<extension>\.[^.]+?|)$";

        [GeneratedRegex(ProjectFileNamePattern)]
        private static partial Regex ProjectFileNameRegex();

        [GeneratedRegex(ProjectNamePattern)]
        private static partial Regex ProjectNameRegex();

        public static string ProjectsPath { get; } = Path.Combine(Path.GetTempPath(), "pva.SuperV");
        public static Project? CurrentProject { get; set; }
        public Dictionary<String, Class> Classes { get; init; } = new(StringComparer.OrdinalIgnoreCase);

        private string _name;

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        /// <value>
        /// The project name.
        /// </value>
        public String Name
        {
            get { return _name; }
            set
            {
                ValidateName(value);
                _name = value;
            }
        }

        public string Description { get; set; }

        public int Version { get; set; }

        public static WipProject CreateProject(String projectName)
        {
            WipProject project = new(projectName);
            return project;
        }

        public static WipProject CreateProject(RunnableProject runnableProject)
        {
            return new WipProject(runnableProject);
        }

        private static void ValidateName(string name)
        {
            if (!ProjectNameRegex().IsMatch(name))
            {
                throw new InvalidProjectNameException(name, ProjectNamePattern);
            }
        }

        public Class? FindClass(String className)
        {
            try
            {
                return GetClass(className);
            }
            catch { return null; }
        }

        public String GetAssemblyFileName()
        {
            if (!Directory.Exists(ProjectsPath))
            {
                Directory.CreateDirectory(ProjectsPath);
            }
            // TODO Add version in assembly name
            return Path.Combine(ProjectsPath, $"{Name}-V{Version}.dll");
        }

        public Class GetClass(String className)
        {
            if (Classes.TryGetValue(className, out Class? value))
            {
                return value;
            }

            throw new UnknownClassException(className);
        }

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

        protected int GetNextVersion()
        {
            return GetProjectHighestVersion(Name) + 1;
        }

        public virtual void Unload()
        {
            Classes.Clear();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Unload();
        }
    }
}