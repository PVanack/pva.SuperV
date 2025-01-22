using pva.Helpers.Extensions;
using pva.SuperV.Engine.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pva.SuperV.Engine.Processing
{
    public class HistorizationProcessing<T> : FieldValueProcessing<T>
    {
        private HistoryRepository? _historyRepository;
        private FieldDefinition<DateTime>? _timestampField;

        private List<IFieldDefinition> FieldsToHistorize { get; } = [];

        /// <summary>
        /// The class time serie ID returned from history storage.
        /// </summary>
        private string? _classTimeSerieId;

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
            _classTimeSerieId = _historyRepository?.UpsertClassTimeSerie(project.Name!, clazz.Name!, this);
        }

        private void ValidateParameters(Project project, Class clazz, string trigerringFieldName, string historyRepositoryName, string? timestampFieldName, List<string> fieldsToHistorize)
        {
            TrigerringFieldDefinition = GetFieldDefinition<T>(clazz, trigerringFieldName);
            if (!String.IsNullOrEmpty(timestampFieldName))
            {
                _timestampField = GetFieldDefinition<DateTime>(clazz, timestampFieldName);
            }
            if (project.HistoryRepositories.TryGetValue(historyRepositoryName, out var repository))
            {
                _historyRepository = repository;
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
            for (int index = 3; index < CtorArguments.Count; index++)
            {
                fieldsToHistorize.Add(GetCtorArgument<string>(index)!);
            }
            ValidateParameters(project, clazz, trigerringFieldName, historyRepositorydName, timestampFieldName, fieldsToHistorize);
            _classTimeSerieId = _historyRepository?.UpsertClassTimeSerie(project.Name!, clazz.Name!, this);
        }

        public override void ProcessValue(IInstance instance, Field<T> changedField, bool valueChanged, T previousValue, T currentValue)
        {
            DateTime? historyTs;
            if (_timestampField != null) {
                Field<DateTime>? timestamp = GetInstanceField<DateTime>(instance, _timestampField?.Name) ;
                historyTs = timestamp?.Value;
            }
            else {
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
            _historyRepository?.HistorizeValues(_classTimeSerieId!, instance, historyTs ?? DateTime.Now, fieldsToHistorize);
        }
    }
}
