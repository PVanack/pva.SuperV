using System.Reflection.Emit;
using System.Reflection;
using pva.SuperVAccess.Exceptions;

namespace pva.SuperVAccess
{
    /// <summary>
    /// SuperV Project class. It contains all the information required (classes, objects, processing0.
    /// </summary>
    public class Project
    {
        public static Project? CurrentProject { get; private set; }
        public AssemblyBuilder? AssemblyBuilder { get; private set; }
        public ModuleBuilder? ModuleBuilder { get; private set; }
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
            project.AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            // Create a dynamic module in Dynamic Assembly.
            project.ModuleBuilder = project.AssemblyBuilder.DefineDynamicModule(projectName);
            Project.CurrentProject = project;
            return project;
        }

        public Class AddClass(String className)
        {
            if (Classes.ContainsKey(className))
            {
                throw new ClassAlreadyExistException(className);
            }

            TypeBuilder? typeBuilder = ModuleBuilder?.DefineType(className, TypeAttributes.Public);
            Class clazz = new(className, typeBuilder!);
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