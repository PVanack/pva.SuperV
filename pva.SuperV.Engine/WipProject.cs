using pva.Helpers.Extensions;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.Processing;

namespace pva.SuperV.Engine
{
    /// <summary>
    /// WIP (Work In Progress) project. It allows adding/changing/removing <see cref="Class"/>, <see cref="FieldDefinition{T}"/>, but can' add/change/remove instances.
    /// </summary>
    /// <seealso cref="pva.SuperV.Engine.Project" />
    public class WipProject : Project
    {
        /// <summary>
        /// To be loaded instances when the project is converted to a <see cref="RunnableProject"/> through <see cref="Project.Build(WipProject)"/>.
        /// </summary>
        public Dictionary<string, Instance> ToLoadInstances { get; } = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="WipProject"/> class.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        public WipProject(string projectName)
        {
            Name = projectName;
            Version = GetNextVersion();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WipProject"/> class from a <see cref="RunnableProject"/>.
        /// </summary>
        /// <param name="runnableProject">The runnable project.</param>
        public WipProject(RunnableProject runnableProject)
        {
            Name = runnableProject.Name;
            Description = runnableProject.Description;
            Version = GetNextVersion();
            Classes = new(runnableProject.Classes.Count);
            runnableProject.Classes
                .ForEach((k, v) => Classes.Add(k, v.Clone()));
            FieldFormatters = new(runnableProject.FieldFormatters);
            ToLoadInstances = new(runnableProject.Instances, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Adds a new class to the project.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <returns>The newly created <see cref="Class"/>.</returns>
        /// <exception cref="pva.SuperV.Engine.Exceptions.EntityAlreadyExistException"></exception>
        public Class AddClass(string className)
        {
            if (Classes.ContainsKey(className))
            {
                throw new EntityAlreadyExistException("Class", className);
            }

            Class clazz = new(className);
            Classes.Add(className, clazz);
            return clazz;
        }

        /// <summary>
        /// Adds a new class with inheritance to the project.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="baseClassName">Name of the base class.</param>
        /// <returns>The newly created <see cref="Class"/>.</returns>
        /// <exception cref="pva.SuperV.Engine.Exceptions.EntityAlreadyExistException"></exception>
        public Class AddClass(string className, string? baseClassName)
        {
            Class? baseClass = baseClassName is null ? null : GetClass(baseClassName);
            if (Classes.ContainsKey(className))
            {
                throw new EntityAlreadyExistException("Class", className);
            }

            Class clazz = new(className, baseClass);
            Classes.Add(className, clazz);
            return clazz;
        }

        /// <summary>
        /// Removes a class from project.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        public void RemoveClass(string className)
        {
            if (Classes.Remove(className, out var clazz))
            {
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
            Class clazz = GetClass(className);
            return clazz.AddField(field);
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
            Class clazz = GetClass(className);
            FieldFormatter formatter = GetFormatter(formatterName);
            formatter.ValidateAllowedType(typeof(T));
            return clazz.AddField(field, formatter);
        }

        /// <summary>
        /// Removes a field from a class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="fieldName">Name of the field.</param>
        public void RemoveField(string className, string fieldName)
        {
            Class clazz = GetClass(className);
            clazz.RemoveField(fieldName);
        }

        /// <summary>
        /// Adds a field formatter to project to be later used for a specific field formatting (<see cref="AddField{T}(string, FieldDefinition{T}, string)"/>.
        /// </summary>
        /// <param name="fieldFormatter">The field formatter.</param>
        /// <exception cref="pva.SuperV.Engine.Exceptions.EntityAlreadyExistException"></exception>
        public void AddFieldFormatter(FieldFormatter fieldFormatter)
        {
            if (FieldFormatters.ContainsKey(fieldFormatter.Name!))
            {
                throw new EntityAlreadyExistException("Field formatter", fieldFormatter.Name);
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
        /// Adds a field change post processing on a field..
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="className">Name of the class.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldValueProcessing">The field value processing.</param>
        public void AddFieldChangePostProcessing<T>(string className, string fieldName, FieldValueProcessing<T> fieldValueProcessing)
        {
            Class clazz = GetClass(className);
            clazz.AddFieldChangePostProcessing(fieldName, fieldValueProcessing);
        }

        public override string GetId()
        {
            return $"{Name!}-WIP";
        }

        /// <summary>
        /// Adds a history repository to project./>.
        /// </summary>
        /// <param name="historyRepository">The history repository.</param>
        /// <exception cref="pva.SuperV.Engine.Exceptions.EntityAlreadyExistException"></exception>
        public void AddHistoryRepository(HistoryRepository historyRepository)
        {
            if (HistoryRepositories.ContainsKey(historyRepository.Name))
            {
                throw new EntityAlreadyExistException("History repository", historyRepository.Name);
            }

            historyRepository.HistoryStorageEngine = HistoryStorageEngine;
            HistoryRepositories.Add(historyRepository.Name, historyRepository);
        }

        /// <summary>
        /// Removes a history repository.
        /// </summary>
        /// <param name="historyRepositoryName">Name of the history repository.</param>
        public void RemoveHistoryRepository(string historyRepositoryName)
        {
            HistoryRepositories.Remove(historyRepositoryName);
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