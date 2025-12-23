using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.HistoryStorage;

namespace pva.SuperV.Engine.Processing
{
    /// <summary>
    /// Field value historization processing.
    /// </summary>
    /// <typeparam name="T">Type of field on which historization processing is applied.</typeparam>
    public class HistorizationProcessing<T> : FieldValueProcessing<T>, IHistorizationProcessing
    {
        /// <summary>
        /// Associated history repository.
        /// </summary>
        public HistoryRepository? HistoryRepository { get; set; }

        /// <summary>
        /// Field providing the timestamp of time serie.
        /// </summary>
        public FieldDefinition<DateTime>? TimestampFieldDefinition { get; set; }

        /// <summary>
        /// List of fields whose value is to be historized.
        /// </summary>
        public List<IFieldDefinition> FieldsToHistorize { get; } = [];

        /// <summary>
        /// The class time serie ID returned from history storage.
        /// </summary>
        public string? ClassTimeSerieId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HistorizationProcessing{T}"/> class. Used for deserialization.
        /// </summary>
        public HistorizationProcessing()
        {
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="name">Name of instance.</param>
        /// <param name="project">Project.</param>
        /// <param name="clazz">Class.</param>
        /// <param name="trigerringFieldName">Field trigerring the processing.</param>
        /// <param name="historyRepositoryName">Name of history repository (<see cref="HistoryRepository"/>).</param>
        /// <param name="timestampFieldName">Name of the timestamp field. If null, the timestamp of tirgerring field zill be used.</param>
        /// <param name="fieldsToHistorize">List of fields to historize.</param>
        public HistorizationProcessing(string name, Project project, Class clazz, string trigerringFieldName,
            string historyRepositoryName, string? timestampFieldName, List<string> fieldsToHistorize)
            : base(name)
        {
            CtorArguments.Add(trigerringFieldName);
            CtorArguments.Add(historyRepositoryName);
            CtorArguments.Add(timestampFieldName ?? "");
            fieldsToHistorize.ForEach(fieldtoHistorize =>
                CtorArguments.Add(fieldtoHistorize));
            ValidateParameters(project, clazz, trigerringFieldName, historyRepositoryName, timestampFieldName, fieldsToHistorize);
        }

        /// <summary>
        /// Validate the processing parameters.
        /// </summary>
        /// <param name="project">Project.</param>
        /// <param name="clazz">Class.</param>
        /// <param name="trigerringFieldName">Field trigerring the processing.</param>
        /// <param name="historyRepositoryName">Name of history repository (<see cref="HistoryRepository"/>).</param>
        /// <param name="timestampFieldName">Name of the timestamp field. If null, the timestamp of tirgerring field zill be used.</param>
        /// <param name="fieldsToHistorize">List of fields to historize.</param>
        /// <exception cref="UnknownEntityException"></exception>
        private void ValidateParameters(Project project, Class clazz, string trigerringFieldName, string historyRepositoryName, string? timestampFieldName, List<string> fieldsToHistorize)
        {
            TrigerringFieldDefinition = GetFieldDefinition<T>(clazz, trigerringFieldName);
            if (!string.IsNullOrEmpty(timestampFieldName))
            {
                TimestampFieldDefinition = GetFieldDefinition<DateTime>(clazz, timestampFieldName);
            }
            if (project.HistoryRepositories.TryGetValue(historyRepositoryName, out var repository))
            {
                HistoryRepository = repository;
            }
            else
            {
                throw new UnknownEntityException("History repository", historyRepositoryName);
            }
            fieldsToHistorize.ForEach(fieldToHistorize =>
            {
                IFieldDefinition? fieldDefinition = GetFieldDefinition(clazz, fieldToHistorize);
                if (fieldDefinition is not null)
                {
                    FieldsToHistorize.Add(fieldDefinition);
                }
            });
        }

        /// <summary>
        /// Build the processing after deserialization.
        /// </summary>
        /// <param name="project">Project.</param>
        /// <param name="clazz">Class.</param>
        public override void BuildAfterDeserialization(Project project, Class clazz)
        {
            string trigerringFieldName = GetCtorArgument<string>(0)!;
            string historyRepositoryName = GetCtorArgument<string>(1)!;
            string? timestampFieldName = GetCtorArgument<string?>(2);
            List<string> fieldsToHistorize = [];
            for (int index = 3; index < CtorArguments.Count - 1; index++)
            {
                fieldsToHistorize.Add(GetCtorArgument<string>(index)!);
            }
            ClassTimeSerieId = GetCtorArgument<string?>(CtorArguments.Count - 1);
            ValidateParameters(project, clazz, trigerringFieldName, historyRepositoryName, timestampFieldName, fieldsToHistorize);
        }

        public bool IsUsingRepository(string historyRepositoryName)
            => HistoryRepository?.Name.Equals(historyRepositoryName) == true;

        public override bool IsFieldUsed(string fieldName)
            => (TimestampFieldDefinition?.Name.Equals(fieldName) == true)
                || FieldsToHistorize.Any(field => field.Name.Equals(fieldName));

        /// <summary>
        /// Upserts the time series in history storage.
        /// </summary>
        /// <param name="projectName">Name of project.</param>
        /// <param name="className">Name of class.</param>
        public void UpsertInHistoryStorage(string projectName, string className)
        {
            ClassTimeSerieId = HistoryRepository?.UpsertClassTimeSerie(projectName, className, this);
            CtorArguments.Add(ClassTimeSerieId!);
        }

        /// <summary>
        /// Processes the trigerring field value change.
        /// </summary>
        /// <param name="instance">Instance whose field has changed.</param>
        /// <param name="changedField">Changed field.</param>
        /// <param name="valueChanged">Indicates if the value changed or not.</param>
        /// <param name="previousValue">Previous value 9i.e. before change).</param>
        /// <param name="currentValue">New value.</param>
        public override void ProcessValue(IInstance instance, Field<T> changedField, bool valueChanged, T previousValue, T currentValue)
        {
            DateTime? historyTs;
            if (TimestampFieldDefinition != null)
            {
                Field<DateTime>? timestamp = GetInstanceField<DateTime>(instance, TimestampFieldDefinition?.Name);
                historyTs = timestamp?.Value;
            }
            else
            {
                historyTs = changedField.Timestamp.GetValueOrDefault();
            }
            List<IField> fieldsToHistorize = [];
            FieldsToHistorize.ForEach(fieldToHistorize =>
            {
                IField? field = GetInstanceField(instance, fieldToHistorize.Name);
                if (field != null)
                {
                    fieldsToHistorize.Add(field);
                }
            });
            HistoryRepository?.HistorizeValues(this.Name, ClassTimeSerieId!, instance, historyTs ?? DateTime.Now, changedField.Quality, fieldsToHistorize);
        }
    }
}
