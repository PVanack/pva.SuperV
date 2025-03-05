using System.Numerics;

namespace pva.SuperV.Engine.Processing
{
    /// <summary>
    /// Alarm state generation processing on a field.
    /// It sets the alarm state based on a 2 or 4 limits with optional deadband and optionally sets an acknowledgement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="FieldValueProcessing{T}" />
    public class AlarmStateProcessing<T> : FieldValueProcessing<T>, IAlarmStateProcessing where T : INumber<T>
    {
        /// <summary>
        /// The high-high alarm state value.
        /// </summary>
        private const int HighHighAlarmState = 2;

        /// <summary>
        /// The high alarm state value.
        /// </summary>
        private const int HighAlarmState = 1;

        /// <summary>
        /// The ok alarm state value
        /// </summary>
        private const int OkAlarmState = 0;

        /// <summary>
        /// The low alarm state value.
        /// </summary>
        private const int LowAlarmState = -1;

        /// <summary>
        /// The low-low alarm state value.
        /// </summary>
        private const int LowLowAlarmState = -2;

        /// <summary>
        /// The acknowledge state value
        /// </summary>
        private const int AckState = 0;

        /// <summary>
        /// The unacknowledge state value.
        /// </summary>
        private const int UnackState = 1;

        /// <summary>
        /// Gets or sets the high-high limit field definition.
        /// </summary>
        /// <value>
        /// The high-high limit field definition.
        /// </value>
        public IFieldDefinition? HighHighLimitField { get; set; }

        /// <summary>
        /// Gets or sets the high limit field definition.
        /// </summary>
        /// <value>
        /// The high limit field definition.
        /// </value>
        public IFieldDefinition? HighLimitField { get; set; }

        /// <summary>
        /// Gets or sets the low limit field definition.
        /// </summary>
        /// <value>
        /// The low limit field definition.
        /// </value>
        public IFieldDefinition? LowLimitField { get; set; }

        /// <summary>
        /// Gets or sets the low-low limit field definition.
        /// </summary>
        /// <value>
        /// The low-low limit field definition.
        /// </value>
        public IFieldDefinition? LowLowLimitField { get; set; }

        /// <summary>
        /// Gets or sets the deadband field definition.
        /// </summary>
        /// <value>
        /// The deadband field definition.
        /// </value>
        public IFieldDefinition? DeadbandField { get; set; }

        /// <summary>
        /// Gets or sets the alarm state field definition.
        /// </summary>
        /// <value>
        /// The alarm state field definition.
        /// </value>
        public IFieldDefinition? AlarmStateField { get; set; }

        /// <summary>
        /// Gets or sets the acknowledgement state field definition.
        /// </summary>
        /// <value>
        /// The acknowledgement state field definition.
        /// </value>
        public IFieldDefinition? AckStateField { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlarmStateProcessing{T}"/> class. Used for deserialization.
        /// </summary>
        public AlarmStateProcessing()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlarmStateProcessing{T}"/> class.
        /// </summary>
        /// <param name="name">The name of processing.</param>
        /// <param name="clazz">The class of the instance to be processed.</param>
        /// <param name="trigerringFieldName">Name of the trigerring field.</param>
        /// <param name="highHighLimitFieldName">Name of the high-high limit field. Can be null/empty if not used.</param>
        /// <param name="highLimitFieldName">Name of the high limit field.</param>
        /// <param name="lowLimitFieldName">Name of the low limit field.</param>
        /// <param name="lowLowLimitFieldName">Name of the low-low limit field. Can be null/empty if not used.</param>
        /// <param name="deadbandFieldName">Name of the deadband field. Can be null/empty if not used.</param>
        /// <param name="alarmStateFieldName">Name of the alarm state field.</param>
        /// <param name="ackStateFieldName">Name of the acknowledgment state field. Can be null/empty if not used</param>
        public AlarmStateProcessing(string name, Class clazz, string trigerringFieldName,
            string? highHighLimitFieldName, string highLimitFieldName, string lowLimitFieldName, string? lowLowLimitFieldName,
            string? deadbandFieldName, string alarmStateFieldName, string? ackStateFieldName)
            : base(name)
        {
            CtorArguments.Add(trigerringFieldName);
            CtorArguments.Add(highHighLimitFieldName ?? "");
            CtorArguments.Add(highLimitFieldName);
            CtorArguments.Add(lowLimitFieldName);
            CtorArguments.Add(lowLowLimitFieldName ?? "");
            CtorArguments.Add(deadbandFieldName ?? "");
            CtorArguments.Add(alarmStateFieldName);
            CtorArguments.Add(ackStateFieldName ?? "");

            ValidateParameters(clazz, trigerringFieldName,
                highHighLimitFieldName, highLimitFieldName, lowLimitFieldName, lowLowLimitFieldName, deadbandFieldName,
                alarmStateFieldName, ackStateFieldName);
        }

        /// <summary>
        /// Builds the field value processing from the <see cref="P:pva.SuperV.Engine.FieldValueProcessing`1.CtorArguments" /> after deserialization.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="clazz">The clazz.</param>
        public override void BuildAfterDeserialization(Project project, Class clazz)
        {
            string? trigerringFieldName = GetCtorArgument<string>(0);
            string? highHighLimitFieldName = GetCtorArgument<string>(1);
            string? highLimitFieldName = GetCtorArgument<string>(2);
            string? lowLimitFieldName = GetCtorArgument<string>(3);
            string? lowLowLimitFieldName = GetCtorArgument<string>(4);
            string? deadbandFieldName = GetCtorArgument<string>(5);
            string? alarmStateFieldName = GetCtorArgument<string>(6);
            string? ackStateFieldName = GetCtorArgument<string>(7);

            ValidateParameters(clazz, trigerringFieldName,
                highHighLimitFieldName, highLimitFieldName, lowLimitFieldName, lowLowLimitFieldName, deadbandFieldName,
                alarmStateFieldName, ackStateFieldName);
        }

        private void ValidateParameters(Class clazz, string? trigerringFieldName, string? highHighLimitFieldName, string? highLimitFieldName, string? lowLimitFieldName, string? lowLowLimitFieldName, string? deadbandFieldName, string? alarmStateFieldName, string? ackStateFieldName)
        {
            TrigerringFieldDefinition = GetFieldDefinition<T>(clazz, trigerringFieldName);
            HighHighLimitField = GetFieldDefinition<T>(clazz, highHighLimitFieldName);
            HighLimitField = GetFieldDefinition<T>(clazz, highLimitFieldName)!;
            LowLimitField = GetFieldDefinition<T>(clazz, lowLimitFieldName)!;
            LowLowLimitField = GetFieldDefinition<T>(clazz, lowLowLimitFieldName);
            DeadbandField = GetFieldDefinition<T>(clazz, deadbandFieldName);
            AlarmStateField = GetFieldDefinition<int>(clazz, alarmStateFieldName)!;
            AckStateField = GetFieldDefinition<int>(clazz, ackStateFieldName)!;
        }

        /// <summary>
        /// Processes the value change.
        /// </summary>
        /// <param name="instance">Instance on which the triggering field changed.</param>
        /// <param name="changedField">The <see cref="Field{T}" /> which changed.</param>
        /// <param name="valueChanged">If <c>true</c>, indicates that the trigerring field value changed.</param>
        /// <param name="previousValue">The previous value of field.</param>
        /// <param name="currentValue">The current value of field.</param>
        public override void ProcessValue(IInstance instance, Field<T> changedField, bool valueChanged, T previousValue, T currentValue)
        {
            if (!valueChanged)
            {
                return;
            }
            Field<T>? highHighLimit = GetInstanceField<T>(instance, HighHighLimitField?.Name);
            Field<T>? highLimit = GetInstanceField<T>(instance, HighLimitField!.Name);
            Field<T>? lowLimit = GetInstanceField<T>(instance, LowLimitField!.Name);
            Field<T>? lowLowLimit = GetInstanceField<T>(instance, LowLowLimitField?.Name);
            Field<T>? deadband = GetInstanceField<T>(instance, DeadbandField?.Name);
            Field<int> alarmState = GetInstanceField<int>(instance, AlarmStateField!.Name)!;
            Field<int>? ackState = GetInstanceField<int>(instance, AckStateField?.Name);
            int previousAlarmState = alarmState.Value;
            int newAlarmState;
            // TODO: Handle deadband
            if (highHighLimit is not null && currentValue >= highHighLimit.Value)
            {
                newAlarmState = HighHighAlarmState;
            }
            else if (highHighLimit is not null && currentValue < highHighLimit.Value && currentValue >= highLimit!.Value)
            {
                newAlarmState = HighAlarmState;
            }
            else if (lowLowLimit is not null && currentValue <= lowLowLimit.Value)
            {
                newAlarmState = LowLowAlarmState;
            }
            else if (currentValue <= lowLimit!.Value)
            {
                newAlarmState = LowAlarmState;
            }
            else
            {
                newAlarmState = OkAlarmState;
            }

            if (newAlarmState == previousAlarmState)
            {
                return;
            }

            alarmState.SetValue(newAlarmState);
            if (ackState is not null && newAlarmState != OkAlarmState && ackState.Value != UnackState)
            {
                ackState.SetValue(UnackState);
            }
        }
    }
}
