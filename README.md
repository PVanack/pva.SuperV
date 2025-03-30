<a name="readme-top"></a>
<!--
*** Thanks for using Document My Project. (https://github.com/luisvent/document_my_project) 
*** If you have a suggestion that would make this better, please fork  
*** the repo and create a pull request or simply open an issue.
*** Don't forget to give the project a star!
-->

<p align="center"><a href="https://github.com/PVanack/pva.SuperV/graphs/contributors"><img src="https://img.shields.io/github/contributors/PVanack/pva.SuperV.svg?style=for-the-badge" alt="Contributors"></a>
        <a href="https://github.com/PVanack/pva.SuperV/network/members"><img src="https://img.shields.io/github/forks/PVanack/pva.SuperV.svg?style=for-the-badge" alt="Forks"></a>
        <a href="https://github.com/PVanack/pva.SuperV/stargazers"><img src="https://img.shields.io/github/stars/PVanack/pva.SuperV.svg?style=for-the-badge" alt="Stargazers"></a>
        <a href="https://github.com/PVanack/pva.SuperV/issues"><img src="https://img.shields.io/github/issues/PVanack/pva.SuperV.svg?style=for-the-badge" alt="Issues"></a></p><br/>


<div align="center">



# pva.SuperV

pva.SuperV solution aims at creating a dynamic "real time" database along with processings and historization of values.

</div>


<!-- LINKS_PLACEHOLDER -->

<!-- TABLE_CONTENT_PLACEHOLDER -->

## About the Project

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
`AddField(string className, IFieldDefinition field, string formatterName)`. The field definition should be defined by `FieldDefinition<T>` where `T` is a basic type (see [FieldDefinitions](#Field-definitions) for handled types).
- Add fields value processings to the created fields by calling `wipProject.AddFieldChangePostProcessing(string className, string fieldName, IFieldValueProcessing fieldValueProcessing)`. `fieldValueProcessing` can be one of the following:
	- [AlarmStateProcessing](/pva.SuperV.Engine/Processing/FieldValueProcessing.md#Alarm-state-processing)  to check the value against 2 or 4 thresholds and set an alarm state and optionally an acknowledgment field.
	- [HistorizationProcessing](/pva.SuperV.Engine/Processing/FieldValueProcessing.md#Historization-processing) to historize a list of fields to the history storage engine if one is defined in project.
- Build the WIP project to create a runnable project by calling `Project.BuildAsync(wipProject)`. This creates a dynamic assembly
with the [classes](/pva.SuperV.Engine/Class.cs)
and [defined fields](/pva.SuperV.Engine/FiedldDefinitions.cs).
- Create instances from a defined class by calling `runnableProject.CreateInstance(string className, string instanceName)`.
[The created instance](/pva.SuperV.Engine/Instance.cs) will then have the fields and processings associated with the class
they are defined against.
- Write values with their associated quality and timestamp to instances fields through one of the
`runnableProject.SetInstanceValue<T>(string instanceName, string fieldName, T value)` overloads.
- If changes to the structure of a runnable project need to be done, a new WIP project must be created by calling
`Project.CreateProject(runnableProject)`. This WIP project's structure can then modified by going through the above workflow.
- Projects structure can be saved to and restored from file by calling `ProjectStorage,SaveProjectDefinition<T>(T project)` and
`ProjectStorage,LoadProjectDefinition<T>(string filename)`.
Runnable projects instances can be saved to and restored from file by calling
`ProjectStorage,SaveProjectInstances(RunnableProject runnableProject)` and
`ProjectStorage,LoadProjectInstances(RunnableProject runnableProject, string filename)`.

Projects, when no more used, can be unloaded from memory by calling `Project.Unload(Project project)`.

## Field definitions
The engine allows to define [fields](/pva.SuperV.Engine/FiedldDefinitions.cs) of any type,
but the API needs to have concrete type not generics ones, so changes need to be done in the following files should you 
want to add a new field type:
- [FieldValueSetter.SetValue()](/pva.SuperV.Engine/FieldValueSetter.cs)
- [TDengineHistoryStorage](/pva.SuperV.Engine/HistoryStorage/TDengineHistoryStorage.cs)
- [FieldDefinitionMapper](/pva.SuperV.Model/FieldDefinitions/FieldDefinitionMapper.cs)
- [FieldValueMapper](/pva.SuperV.Model/Instances/FieldValueMapper.cs)
- [HistoryRowMapper](/pva.SuperV.Model/HistoryRetrieval/HistoryRowMapper.cs)

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


![Ubuntu build](https://github.com/PVanack/pva.SuperV/actions/workflows/dotnet-ubuntu.yml/badge.svg?event=push)

## Technical Stack
- [![C#][C#-badge]][C#-url] - All programming is done in C#
- [![TDengine][TDengine-badge]][TDengine-url] - TDengine OSS is used as the data historian

[C#-badge]: https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp
[C#-url]: https://dotnet.microsoft.com/
[TDengine-badge]: https://eujqw4hwudm.exactdn.com/wp-content/uploads/29.01-01-logo-white.svg
[TDengine-url]: https://tdengine.com/oss/

## ️Setup

### Installation

To install this project, follow these steps:

1. Install




## About the Author

**Patrice VANACKER**

This project was created by Patrice VANACKER. Connect with me on [GitHub](https://github.com/pVanack)  to learn more about my projects and professional background.


## License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).


<p align="right"><a href="#readme-top">(Back to top)</a></p>

---
 <div align="center">Built with ❤️ with <a href="https://github.com/luisvent/document_my_project">Document My Project</a></div>





