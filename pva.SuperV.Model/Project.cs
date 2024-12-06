using pva.SuperV.Model.Exceptions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace pva.SuperV.Model
{
    /// <summary>
    /// SuperV Project class. It contains all the information required (classes, objects, processing).
    /// </summary>
    public partial class Project
    {
        private const string ProjectNamePattern = "^([A-Z]|[a-z]|[0-9])*$";

        [GeneratedRegex(ProjectNamePattern)]
        private static partial Regex ProjectNameRegex();

        public static Project? CurrentProject { get; set; }
        public Dictionary<String, Class> Classes { get; init; } = [];

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

        private static void ValidateName(string value)
        {
            if (!ProjectNameRegex().IsMatch(value))
            {
                throw new InvalidProjectNameException(value, ProjectNamePattern);
            }
        }

        public static WipProject CreateProject(String projectName)
        {
            var project = new WipProject
            {
                Name = projectName
            };
            return project;
        }

        public static WipProject CreateProject(RunnableProject runnableProject)
        {
            return new WipProject(runnableProject);
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
            // TODO Add version in assembly name
            return Path.Combine(Path.GetTempPath(), $"{Name}.dll");
        }

        public Class GetClass(String className)
        {
            if (Classes.TryGetValue(className, out Class? value))
            {
                return value;
            }

            throw new UnknownClassException(className);
        }
    }
}