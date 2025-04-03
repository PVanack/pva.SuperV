using pva.Helpers.Extensions;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.FieldFormatters;
using pva.SuperV.Engine.HistoryStorage;
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
        /// To be loaded instances when the project is converted to a <see cref="RunnableProject"/> through <see cref="Project.BuildAsync(WipProject)"/>.
        /// </summary>
        public Dictionary<string, Instance> ToLoadInstances { get; } = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="WipProject"/> class. Only used for deserialization.
        /// </summary>
        public WipProject()
        {
        }

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
            HistoryStorageEngineConnectionString = runnableProject.HistoryStorageEngineConnectionString;
            HistoryStorageEngine = HistoryStorageEngineFactory.CreateHistoryStorageEngine(runnableProject.HistoryStorageEngineConnectionString);
            HistoryRepositories = new(runnableProject.HistoryRepositories);
            HistoryRepositories.Values.ForEach(repository
                => repository.HistoryStorageEngine = HistoryStorageEngine);
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
        /// Updates a class.
        /// </summary>
        /// <param name="className">Name of class to be updated</param>
        /// <param name="baseClassName">Base class name</param>
        /// <returns>Updated class.</returns>
        /// <exception cref="UnknownEntityException"></exception>
        public Class UpdateClass(string className, string? baseClassName)
        {
            if (Classes.TryGetValue(className, out Class? clazz))
            {
                Class? baseClass = null;
                if (!String.IsNullOrEmpty(baseClassName) && !Classes.TryGetValue(className, out baseClass))
                {
                    throw new UnknownEntityException("Class", baseClassName);
                }
                clazz.BaseClassName = baseClassName;
                clazz.BaseClass = baseClass;
                return clazz;
            }
            throw new UnknownEntityException("Class", className);
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
                    .Where(instance => instance.Class.Name!.Equals(clazz.Name))
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
        public IFieldDefinition AddField(string className, IFieldDefinition field)
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
        public IFieldDefinition AddField(string className, IFieldDefinition field, string? formatterName)
        {
            Class clazz = GetClass(className);
            FieldFormatter? formatter = GetFormatter(formatterName);
            formatter?.ValidateAllowedType(field.Type);
            return clazz.AddField(field, formatter);
        }

        public IFieldDefinition UpdateField(string className, string fieldName, IFieldDefinition field, string? formatterName)
        {
            Class clazz = GetClass(className);
            FieldFormatter? formatter = GetFormatter(formatterName);
            formatter?.ValidateAllowedType(field.Type);
            return clazz.UpdateField(fieldName, field, formatter);
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

        public void UpdateFieldFormatter(string fieldFormatterName, FieldFormatter fieldFormatter)
        {
            FieldFormatter? existingFormatter = GetFormatter(fieldFormatterName);
            if (existingFormatter != null)
            {
                if (existingFormatter.GetType() == fieldFormatter.GetType())
                {
                    FieldFormatters[fieldFormatterName] = fieldFormatter;
                    return;
                }
                throw new WrongFieldTypeException(fieldFormatterName, existingFormatter.GetType(), fieldFormatter.GetType());
            }
            throw new UnknownEntityException("Field formatter", fieldFormatterName);
        }


        /// <summary>
        /// Removes a field formatter.
        /// </summary>
        /// <param name="fieldFormatterName">Name of the field formatter.</param>
        public bool RemoveFieldFormatter(string fieldFormatterName)
        {
            VerifyFieldFormatterNotUsed(fieldFormatterName);
            return FieldFormatters.Remove(fieldFormatterName);
        }

        private void VerifyFieldFormatterNotUsed(string fieldFormatterName)
        {
            Classes.Values.ForEach(clazz =>
            {
                List<String> fieldsUsingFormatter = [.. clazz.FieldDefinitions.Values
                    .Where(field => field.Formatter is not null && field.Formatter.Name.Equals(fieldFormatterName))
                    .Select(field => field.Name)];
                if (fieldsUsingFormatter.Count > 0)
                {
                    throw new EntityInUseException("Field formatter", fieldFormatterName, clazz.Name, fieldsUsingFormatter);
                }
            });
        }

        /// <summary>
        /// Adds a field change post processing on a field..
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldValueProcessing">The field value processing.</param>
        public void AddFieldChangePostProcessing(string className, string fieldName, IFieldValueProcessing fieldValueProcessing)
        {
            Class clazz = GetClass(className);
            clazz.AddFieldChangePostProcessing(fieldName, fieldValueProcessing);
        }

        public void UpdateFieldChangePostProcessing(string className, string fieldName, string processingName, IFieldValueProcessing fieldProcessing)
        {
            Class clazz = GetClass(className);
            clazz.UpdateFieldChangePostProcessing(fieldName, processingName, fieldProcessing);
        }

        public void RemoveFieldChangePostProcessing(string className, string fieldName, string processingName)
        {
            Class clazz = GetClass(className);
            clazz.RemoveFieldChangePostProcessing(fieldName, processingName);
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

        public void UpdateHistoryRepository(string historyRepositoryName, HistoryRepository historyRepository)
        {
            if (HistoryRepositories.TryGetValue(historyRepositoryName, out HistoryRepository? _))
            {
                HistoryRepositories[historyRepositoryName] = historyRepository;
                return;
            }
            throw new UnknownEntityException("History repository", historyRepositoryName);
        }

        /// <summary>
        /// Removes a history repository.
        /// </summary>
        /// <param name="historyRepositoryName">Name of the history repository.</param>
        public void RemoveHistoryRepository(string historyRepositoryName)
        {
            VerifyHistoryRepositoryNotUsed(historyRepositoryName);
            HistoryRepositories.Remove(historyRepositoryName);
        }

        private void VerifyHistoryRepositoryNotUsed(string historyRepositoryName)
        {
            Classes.Values.ForEach(clazz =>
            {
                List<String> fieldsUsingHistoryRepository = [.. clazz.FieldDefinitions.Values
                    .Where(field => field.ValuePostChangeProcessings
                        .OfType<IHistorizationProcessing>()
                        .Any(historyValueProcessing => historyValueProcessing.IsUsingRepository(historyRepositoryName)))
                    .Select(field => field.Name)];
                if (fieldsUsingHistoryRepository.Count > 0)
                {
                    throw new EntityInUseException("History Repository", historyRepositoryName, clazz.Name, fieldsUsingHistoryRepository);
                }
            });
        }


        /// <summary>
        /// Clones as <see cref="RunnableProject"/>.
        /// </summary>
        /// <returns><see cref="RunnableProject"/></returns>
        public RunnableProject CloneAsRunnable()
            => new(this);

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