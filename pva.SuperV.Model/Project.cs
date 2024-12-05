using pva.SuperV.Model.Exceptions;
using System.Reflection;
using System.Text;

namespace pva.SuperV.Model
{
    /// <summary>
    /// SuperV Project class. It contains all the information required (classes, objects, processing).
    /// </summary>
    public class Project
    {
        public static Project? CurrentProject { get; set; }
        public Dictionary<String, Class> Classes { get; init; } = [];

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        /// <value>
        /// The project name.
        /// </value>
        public String Name { get; set; } = null!;

        public static WipProject CreateProject(String projectName)
        {
            //TODO Validate project name with regex
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