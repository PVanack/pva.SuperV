# Field value change processing
Field value processing allows to perform custom processing whenever the associated triggering field value changes. All field value processings extend the [FieldValueProcessing\<T\>](/Processing/FieldValueProcessing.cs) where *T* is the type of the field trigerring the change.

They need to implement method `void ProcessValue(IInstance instance, Field<T> changedField, bool valueChanged, T previousValue, T currentValue)`

The following processings are already defined:
- [AlarmStateProcessing](#Alarm-state-processing)
- [HistorizationProcessing](#Historization-processing)

## Alarm state processing
[Alarm state processing](AlarmStateProcessing.cs) allows to update an alarm state field based on 2 or 4 threshold fields and an acknowledgement field if alarm state goes to something other than 0.
Its properties are the following:
- *trigerringFieldName* The name of the field trigerring the alarm state check.
- *highHighLimitFieldName* The name of the field containing the high high limit. Can be null.
- *highLimitFieldName* The name of the field containing the high limit.
- *lowLimitFieldName* The name of the field containing the low limit.
- *lowLowLimitFieldName* The name of the field containing the low low limit. Can be null.
- *deadbandFieldName* The name of the field containing the deadband. Can be null.
- *alarmStateFieldName* The name of the field which will be updated with the alarm state. It needs to be an **integer**. Its values will be:
	- *-2* for low low alarm state (i.e. the trigerring field value is below low low limit).
	- *-1* for low alarm state (i.e. the trigerring field value is between low low and low limits).
	- *0* for normal alarm state (i.e. the trigerring field value is between low and high limits).
	- *1* for high alarm state  (i.e. the trigerring field value is between high and high high limits).
	- *2* for high high alarm state  (i.e. the trigerring field value is above high limit).
- *ackStateFieldName* The name of the field whose value will be set to 1 if the alarm state changes and is not normal (i.e. not 0).

## Historization processing
[Historization processing](HistorizationProcessing.cs) allows to historize a list of fields when the value of the fields trigerring the processing changes. Its properties are the following:
- *trigerringFieldName* The name of the field trigerring the historization.
- *historyRepositoryName* The name of a repository to be used for historizing the fields. It needs to exist in the project.
- *timestampFieldName* The name of the field to be used as timestamp of the history row. If not specified, the timestamp of the tirgerring field is used.
- *fieldsToHistorize* A list of fields which are to be historized.