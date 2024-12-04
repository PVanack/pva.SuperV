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
        public static Project? CurrentProject { get; private set; }
        public Dictionary<String, Class> Classes { get; init; } = [];

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        /// <value>
        /// The project name.
        /// </value>
        public String Name { get; private set; } = null!;

        public static Project CreateProject(String projectName)
        {
            //TODO Validate project name with regex
            var project = new Project
            {
                Name = projectName
            };
            // Create an assembly.
            AssemblyName assemblyName = new()
            {
                Name = projectName
            };
            Project.CurrentProject = project;
            return project;
        }

        public Class AddClass(String className)
        {
            if (Classes.ContainsKey(className))
            {
                throw new ClassAlreadyExistException(className);
            }

            Class clazz = new(className);
            Classes.Add(className, clazz);
            return clazz;
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

        public string GetCode()
        {
            StringBuilder codeBuilder = new();
            codeBuilder.AppendLine($"namespace {Name} {{");
            foreach (var item in Classes)
            {
                codeBuilder.AppendLine(item.Value.GetCode());
            }
            codeBuilder.AppendLine("}");
            return codeBuilder.ToString();
        }

        public dynamic? CreateClassInstance(string className, string instanceName)
        {
            Class clazz = GetClass(className);
            string classFullName = $"{Name}.{clazz.Name}";
            dynamic? instance = Activator.CreateInstanceFrom(GetAssemblyFileName(), classFullName)
                ?.Unwrap();
            instance.Name = instanceName;
            return instance;
        }
    }
}