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
- Create an empty WIP (Work In Progress) project through `Project.CreateProject(String projectName)` or `CreateProject(string projectName, string? historyStorageEngineConnectionString)`. For the historyStorageConnectionString, see [history storage](pva.SuperV.Engine/HistoryStorage/HistoryStorage.md)
- Add history repositories to project if required with `wipProject.AddHistoryRepository(HistoryRepository historyRepository)` if a history storage engine has been defined when creating the project.
- Add field formatters to project if required with `wipProject.AddFieldFormatter(FieldFormatter fieldFormatter)` where fieldFormatter can be be one of the following:
	- [EnumFormatter](/pva.SuperV.Engine/FieldFormatters/FieldFormatter.md#Enum-formatter) which allows to format integer values to strings (i.e. to convert 0/1 values to OFF/ON string)
- Add classes to this project with `wipProject.AddClass(string className)` or `wipProject.AddClass(string className, String baseClassName)`.
- Add fields definitions to the created classes by calling `wpiProject.AddField(string className, IFieldDefinition field)` or
`AddField(string className, IFieldDefinition field, string formatterName)`. The field definition should be defined by `FieldDefinition\<T\>` where `T` is a basic type (see [FieldDefinitions](#Field-definitions) for handled types).
- Add fields value processings to the created fields by calling `wipProject.AddFieldChangePostProcessing(string className, string fieldName, IFieldValueProcessing fieldValueProcessing)`. `fieldValueProcessing` can be on of the following:
	- [AlarmStateProcessing](/pva.SuperV.Engine/Processing/FieldValueProcessing.md#Alarm-state-processing)  to check the value against 2 or 4 thresholds and set an alarm state and optionally an acknowledgment field.
	- [HistorizationProcessing](/pva.SuperV.Engine/Processing/FieldValueProcessing.md#Historization-processing) to historize a list of fields to the history storage engine if one is defined in project.
- Build the WIP project to create a runnable project by calling `Project.BuildAsync(wipProject)`. This creates a dynamic assembly
with the [classes](/pva.SuperV.Engine/Class.cs)
and [defined fields](/pva.SuperV.Engine/FiedldDefinitions.cs).
- Create instances from a defined class by calling `runnableProject.CreateInstance(string className, string instanceName)`.
[The created instance](/pva.SuperV.Engine/Instance.cs) will then have the fields and processings associated with the class
they are defined against.
- Write values with their associated quality and timestamp to instances fields through one of the
`runnableProject.SetInstaceValue\<T\>()` overloads.

If changes to the structure of a runnable project need to be done, a new WIP project must be created by calling
`Project.CreateProject(runnableProject)`. This project's structure can then modified by going through the above workflow.

Projects structure can be saved to and restored from file by calling `ProjectStorage,SaveProjectDefinition\<T\>(T project)` and
`ProjectStorage,LoadProjectDefinition\<T\>(string filename)`.
Runnable projects instances can be saved to and restored from file by calling
`ProjectStorage,SaveProjectInstances(RunnableProject runnableProject)` and
`ProjectStorage,LoadProjectInstances(RunnableProject runnableProject, string filename)`.

Projects can be unloaded from memory by calling `Project.Unload(Project project)`.

## Field definitions
The engine allows to define [fields](/pva.SuperV.Engine/FiedldDefinitions.cs) of any type,
but the API needs to have concrete type not generics ones, so changes need to be done in the following files should you 
want to add a new field type:
- [Field.SetValue()](/pva.SuperV.Engine/Field.cs)
- the [field definition mapper](/pva.SuperV.Model/FieldDefinitions/FieldDefinitionMapper.cs)
- the [field value mapper](/pva.SuperV.Model/Instances/FieldValueMapper.cs)

Current handled types are the following:
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
