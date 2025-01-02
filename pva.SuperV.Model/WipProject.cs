using pva.Helpers.Extensions;
using pva.SuperV.Model.Exceptions;
using System.Text;

namespace pva.SuperV.Model
{
    /// <summary>
    /// WIP (Work In Progress) project. It allows adding/changing/removing <see cref="Class"/>, <see cref="FieldDefinition{T}"/>, but can' add/change/remove instances.
    /// </summary>
    /// <seealso cref="pva.SuperV.Model.Project" />
    public class WipProject : Project
    {
        /// <summary>
        /// To be loaded instances when the project is converted to a <see cref="RunnableProject"/> through <see cref="ProjectBuilder.Build(WipProject)"/>.
        /// </summary>
        public Dictionary<string, dynamic> ToLoadInstances { get; init; } = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="WipProject"/> class.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        public WipProject(string projectName)
        {
            Name = projectName;
            this.Version = GetNextVersion();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WipProject"/> class from a <see cref="RunnableProject"/>.
        /// </summary>
        /// <param name="runnableProject">The runnable project.</param>
        public WipProject(RunnableProject runnableProject)
        {
            this.Name = runnableProject.Name;
            this.Description = runnableProject.Description;
            this.Version = GetNextVersion();
            this.Classes = new(runnableProject.Classes.Count);
            runnableProject.Classes
                .ForEach((k, v) => this.Classes.Add(k, v.Clone()));
            this.FieldFormatters = new(runnableProject.FieldFormatters);
            this.ToLoadInstances = new(runnableProject.Instances, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Adds a new class to the project.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <returns>The newly created <see cref="Class"/>.</returns>
        /// <exception cref="pva.SuperV.Model.Exceptions.ClassAlreadyExistException"></exception>
        public Class AddClass(string className)
        {
            if (Classes.ContainsKey(className))
            {
                throw new ClassAlreadyExistException(className);
            }

            Class clazz = new(className);
            Classes.Add(className, clazz);
            return clazz;
        }

        /// <summary>
        /// Removes a class from project.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        public void RemoveClass(string className)
        {
            if (Classes.TryGetValue(className, out var clazz))
            {
                Classes.Remove(className);
                ToLoadInstances.Values
                    .Where(instance => instance.Class.Name.Equals(clazz.Name))
                    .ToList()
                    .ForEach(instance => ToLoadInstances.Remove(instance.Name));
            }
        }

        /// <summary>
        /// Adds a field to a class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="className">Name of the class.</param>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        public FieldDefinition<T> AddField<T>(string className, FieldDefinition<T> field)
        {
            Class? clazz = GetClass(className);
            return clazz!.AddField<T>(field);
        }

        /// <summary>
        /// Adds a field to a class with a specific field formatter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="className">Name of the class.</param>
        /// <param name="field">The field.</param>
        /// <param name="formatterName">Name of the formatter.</param>
        /// <returns></returns>
        public FieldDefinition<T> AddField<T>(string className, FieldDefinition<T> field, string formatterName)
        {
            Class? clazz = GetClass(className);
            FieldFormatter? formatter = GetFormatter(formatterName);
            formatter.ValidateAllowedType(typeof(T));
            return clazz!.AddField<T>(field, formatter);
        }

        /// <summary>
        /// Removes a field from a class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="fieldName">Name of the field.</param>
        public void RemoveField(string className, string fieldName)
        {
            Class? clazz = GetClass(className);
            clazz!.RemoveField(fieldName);
        }

        /// <summary>
        /// Adds a field formatter to project to be later used for a specific field formatting (<see cref="AddField{T}(string, FieldDefinition{T}, string)"/>.
        /// </summary>
        /// <param name="fieldFormatter">The field formatter.</param>
        /// <exception cref="pva.SuperV.Model.Exceptions.FormatterAlreadyExistException"></exception>
        public void AddFieldFormatter(FieldFormatter fieldFormatter)
        {
            if (FieldFormatters.ContainsKey(fieldFormatter.Name!))
            {
                throw new FormatterAlreadyExistException(fieldFormatter.Name);
            }

            FieldFormatters.Add(fieldFormatter.Name!, fieldFormatter);
        }

        /// <summary>
        /// Removes a field formatter.
        /// </summary>
        /// <param name="fieldFormatterName">Name of the field formatter.</param>
        public void RemoveFieldFormatter(string fieldFormatterName)
        {
            FieldFormatters.Remove(fieldFormatterName);
        }

        /// <summary>
        /// Gets the C# code for generating the project's assembly with <see cref="ProjectBuilder.Build(WipProject)"/>.
        /// </summary>
        /// <returns>C# code.</returns>
        public string GetCode()
        {
            StringBuilder codeBuilder = new();
            codeBuilder.AppendLine($"using {this.GetType().Namespace};");
            codeBuilder.AppendLine("using System.Collections.Generic;");
            codeBuilder.AppendLine("using System.Reflection;");
            codeBuilder.AppendLine("[assembly: AssemblyProduct(\"pva.SuperV\")]");
            codeBuilder.AppendLine($"[assembly: AssemblyTitle(\"{Description}\")]");
            codeBuilder.AppendLine($"[assembly: AssemblyVersion(\"{Version}\")]");
            codeBuilder.AppendLine($"[assembly: AssemblyFileVersion(\"{Version}\")]");
            codeBuilder.AppendLine($"[assembly: AssemblyInformationalVersion(\"{Version}\")]");
            codeBuilder.AppendLine($"namespace {Name}.V{Version} {{");
            Classes
                .ForEach((_, v) => codeBuilder.AppendLine(v.GetCode()));
            codeBuilder.AppendLine("}");
            return codeBuilder.ToString();
        }

        /// <summary>
        /// Clones as <see cref="RunnableProject"/>.
        /// </summary>
        /// <returns><see cref="RunnableProject"/></returns>
        public RunnableProject CloneAsRunnable()
        {
            return new RunnableProject(this);
        }

        /// <summary>
        /// Unloads the project.
        /// </summary>
        public override void Unload()
        {
            ToLoadInstances.Values.ForEach(instance => instance.Dispose());
            ToLoadInstances.Clear();
            base.Unload();
        }
    }
}