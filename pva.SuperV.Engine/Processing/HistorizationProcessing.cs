using pva.Helpers.Extensions;
using pva.SuperV.Engine.Exceptions;

namespace pva.SuperV.Engine.Processing
{
    public class HistorizationProcessing<T> : FieldValueProcessing<T>, IHistorizationProcessing
    {
        public HistoryRepository? HistoryRepository { get; set; }
        public FieldDefinition<DateTime>? TimestampFieldDefinition { get; set; }

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

        private void ValidateParameters(Project project, Class clazz, string trigerringFieldName, string historyRepositoryName, string? timestampFieldName, List<string> fieldsToHistorize)
        {
            TrigerringFieldDefinition = GetFieldDefinition<T>(clazz, trigerringFieldName);
            if (!String.IsNullOrEmpty(timestampFieldName))
            {
                TimestampFieldDefinition = GetFieldDefinition<DateTime>(clazz, timestampFieldName);
            }
            if (project.HistoryRepositories.TryGetValue(historyRepositoryName, out var repository))
            {
                HistoryRepository = repository;
            }
            else
            {
                throw new UnknownHistoryRepositoryException(historyRepositoryName);
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

        public override void BuildAfterDeserialization(Project project, Class clazz)
        {
            string trigerringFieldName = GetCtorArgument<string>(0)!;
            string historyRepositorydName = GetCtorArgument<string>(1)!;
            string? timestampFieldName = GetCtorArgument<string?>(2);
            List<string> fieldsToHistorize = [];
            for (int index = 3; index < CtorArguments.Count - 1; index++)
            {
                fieldsToHistorize.Add(GetCtorArgument<string>(index)!);
            }
            ClassTimeSerieId = GetCtorArgument<string?>(CtorArguments.Count - 1);
            ValidateParameters(project, clazz, trigerringFieldName, historyRepositorydName, timestampFieldName, fieldsToHistorize);
        }

        public void UpsertInHistoryStorage(string projectName, string className)
        {
            ClassTimeSerieId = HistoryRepository?.UpsertClassTimeSerie(projectName, className, this);
            CtorArguments.Add(ClassTimeSerieId!);
        }

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
                IField? field = GetInstanceField(instance, fieldToHistorize?.Name);
                if (field != null)
                {
                    fieldsToHistorize.Add(field);
                }
            });
            HistoryRepository?.HistorizeValues(ClassTimeSerieId!, instance, historyTs ?? DateTime.Now, fieldsToHistorize);
        }
    }
}
