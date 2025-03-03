using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Instances;

namespace pva.SuperV.Api.Services.Instances
{
    public class FieldValueService : BaseService, IFieldValueService
    {
        public FieldValueModel GetField(string projectId, string instanceName, string fieldName)
        {
            return FieldValueMapper.ToDto(GetFieldEntity(projectId, instanceName, fieldName));
        }

        public FieldValueModel UpdateFieldValue(string projectId, string instanceName, string fieldName, FieldValueModel value)
        {
            IField field = GetFieldEntity(projectId, instanceName, fieldName);
            (value switch
            {
                BoolFieldValueModel derivedField => new Action(() => Field<bool>.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                DateTimeFieldValueModel derivedField => new Action(() => Field<DateTime>.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                DoubleFieldValueModel derivedField => new Action(() => Field<double>.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                FloatFieldValueModel derivedField => new Action(() => Field<float>.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                IntFieldValueModel derivedField => new Action(() => Field<int>.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                LongFieldValueModel derivedField => new Action(() => Field<long>.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                ShortFieldValueModel derivedField => new Action(() => Field<short>.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                StringFieldValueModel derivedField => new Action(() => Field<string>.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                TimeSpanFieldValueModel derivedField => new Action(() => Field<TimeSpan>.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                UintFieldValueModel derivedField => new Action(() => Field<uint>.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                UlongFieldValueModel derivedField => new Action(() => Field<ulong>.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                UshortFieldValueModel derivedField => new Action(() => Field<ushort>.SetValue(field, derivedField.Value, derivedField.Timestamp, derivedField.Quality)),
                _ => new Action(() => throw new UnhandledFieldTypeException(field.FieldDefinition!.Name, field.Type))
            })();
            return FieldValueMapper.ToDto(field);
        }
    }
}
