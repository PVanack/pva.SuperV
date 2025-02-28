namespace pva.SuperV.Engine.Processing
{
    public interface IAlarmStateProcessing : IFieldValueProcessing
    {
        /// <summary>
        /// Gets or sets the high-high limit field definition.
        /// </summary>
        /// <value>
        /// The high-high limit field definition.
        /// </value>
        IFieldDefinition? HighHighLimitField { get; set; }

        /// <summary>
        /// Gets or sets the high limit field definition.
        /// </summary>
        /// <value>
        /// The high limit field definition.
        /// </value>
        IFieldDefinition? HighLimitField { get; set; }

        /// <summary>
        /// Gets or sets the low limit field definition.
        /// </summary>
        /// <value>
        /// The low limit field definition.
        /// </value>
        IFieldDefinition? LowLimitField { get; set; }

        /// <summary>
        /// Gets or sets the low-low limit field definition.
        /// </summary>
        /// <value>
        /// The low-low limit field definition.
        /// </value>
        IFieldDefinition? LowLowLimitField { get; set; }

        /// <summary>
        /// Gets or sets the deadband field definition.
        /// </summary>
        /// <value>
        /// The deadband field definition.
        /// </value>
        IFieldDefinition? DeadbandField { get; set; }

        /// <summary>
        /// Gets or sets the alarm state field definition.
        /// </summary>
        /// <value>
        /// The alarm state field definition.
        /// </value>
        IFieldDefinition? AlarmStateField { get; set; }

        /// <summary>
        /// Gets or sets the acknowledgement state field definition.
        /// </summary>
        /// <value>
        /// The acknowledgement state field definition.
        /// </value>
        IFieldDefinition? AckStateField { get; set; }

    }
}
