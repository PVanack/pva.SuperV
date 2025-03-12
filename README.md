![Ubuntu build](https://github.com/PVanack/pva.SuperV/actions/workflows/dotnet-ubuntu.yml/badge.svg?event=push)
# The pva.SuperV project
This project aims at creating a dynamic "real time" database along with processings and historization of values.
It's organized through [projects](/pva.SuperV.Engine/Project.cs)
which could be either [WIP](/pva.SuperV.Engine/WipProject.cs)
(i.e. with structure can be modified) (WIP stands for Work In Progress) or
[runnable](/pva.SuperV.Engine/RunnableProject.cs) (i.e. with a readonly
structure, but where instances of classes can be created).
Along fields in classes, [processing](/pva.SuperV.Engine/Processing/FieldValueProcessing.cs) can be added to process value changes.

![Structure diagram](/drawings/Structure.drawio.png)

## Solution orgnization
The solution is composed of the following projects:
- The [Engine](/pva.SuperV.Engine) which holds all the intelligence
- The [REST API](/pva.SuperV.Api) which allows to access the engine from clients.

## SuperV project workflow
Project creation workflow using the engine library is as follows (the same can be achieved using the [REST API with those calls](/pva.SuperV.Api/pva.SuperV.Api.http):
- Create an empty WIP (Work In Progress) project through <code>Project.CreateProject(String projectName)</code> or <code>CreateProject(string projectName, string? historyStorageEngineConnectionString)</code>. For the historyStorageConnectionString, see [history storage](pva.SuperV.Engine/HistoryStorage/HistoryStorage.md)
- Add history repositories to project if required with <code>wipProject.AddHistoryRepository(HistoryRepository historyRepository)</code> if a history storage engine has been defined when creating the project.
- Add field formatters to project if required with <code>wipProject.AddFieldFormatter(FieldFormatter fieldFormatter)</code> where fieldFormatter can be be one of the following:
	- [EnumFormatter](/pva.SuperV.Engine/FieldFormatters/FieldFormatter.md#Enum-formatter) which allows to format integer values to strings (i.e. to convert 0/1 values to OFF/ON string)
- Add classes to this project with <code>wipProject.AddClass(string className)</code> or <code>wipProject.AddClass(string className, String baseClassName)</code>.
- Add fields definitions to the created classes by calling <code>wpiProject.AddField(string className, IFieldDefinition field)</code> or <code>AddField(string className, IFieldDefinition field, string formatterName)</code>. The field definition should be defined by <code>FieldDefinition\<T\></code> where <code>T</code> is a basic type (see [FieldDefinitions](#Field-definitions) for handled types).
- Add fields value processings to the created fields by calling <code>wipProject.AddFieldChangePostProcessing(string className, string fieldName, IFieldValueProcessing fieldValueProcessing)</code>. <code>fieldValueProcessing</code> can be on of the following:
	- [AlarmStateProcessing](/pva.SuperV.Engine/Processing/FieldValueProcessing.md#Alarm-state-processing)  to check the value against 2 or 4 thresholds and set an alarm state and optionally an acknowledgment field.
	- [HistorizationProcessing](/pva.SuperV.Engine/Processing/FieldValueProcessing.md#Historization-processing) to historize a list of fields to the history storage engine if one is defined in project.
- Build the WIP project to create a runnable project by calling <code>Project.BuildAsync(wipProject)</code>. This creates a dynamic assembly
with the [classes](/pva.SuperV.Engine/Class.cs)
and [defined fields](/pva.SuperV.Engine/FiedldDefinitions.cs).
- Create instances from a defined class by calling <code>runnableProject.CreateInstance(string className, string instanceName)</code>.
[The created instance](/pva.SuperV.Engine/Instance.cs) will then have the fields and processings associated with the class they are defined against.

## Field definitions
The engine allows to define [fields](/pva.SuperV.Engine/FiedldDefinitions.cs) of any type,
but the API needs to have concrete type not generics ones, so
the [field definition mapper](/pva.SuperV.Model/FieldDefinitions/FieldDefinitionMapper.cs) and
[Field.SetValue()](/pva.SuperV.Engine/Field.cs) need 
to be adapted should you want to add a new field type. Current handled types are the following:
- bool
- DateTime
- double
- int
- long
- short
- string
- TimeSpan
- uint
- ulong
- ushort
