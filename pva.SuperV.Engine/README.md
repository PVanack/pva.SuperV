# pva.SuperV.Engine

## Overview
The `pva.SuperV.Engine` project is a core component of the SuperV system, designed to manage and process various projects, classes, instances, and their histories. This project is built on .NET 9.0 and includes several key components and dependencies to facilitate its functionality.

## Project Structure

### Dependencies
- **Microsoft.CodeAnalysis.CSharp**: Provides APIs for working with C# code analysis.
- **TDengine.Connector**: A connector for interacting with TDengine, a time-series database.

### Project References
- **pva.Helpers**: Contains helper functions and utilities used within the `pva.SuperV.Engine` project.

## Key Classes

### Project
The `Project` class is an abstract base class that represents a SuperV project. It contains all the necessary information for managing a project, including classes, instances, and processing logic. Key properties and methods include:
- **ProjectsPath**: The path where all SuperV-related files are stored.
- **CurrentProject**: The currently active project.
- **Name**: The name of the project.
- **Description**: A description of the project.
- **Version**: The version of the project.
- **Classes**: A dictionary of classes within the project.
- **FieldFormatters**: A dictionary of field formatters used in the project.
- **HistoryStorageEngine**: The history storage engine used for storing historical data.
- **HistoryRepositories**: A dictionary of history repositories within the project.
- **Projects**: A concurrent dictionary of all projects in use.
- **CreateProject**: Methods for creating new projects.
- **BuildAsync**: Asynchronously builds a `WipProject` into a `RunnableProject`.
- **GetClass**: Retrieves a class by name.
- **GetFormatter**: Retrieves a field formatter by name.
- **GetAssemblyFileName**: Gets the name of the generated assembly file for the project.
- **Unload**: Unloads the project and clears its resources.
- **Dispose**: Disposes of the project and its resources.

### WipProject
The `WipProject` class represents a Work In Progress (WIP) project. It allows adding, changing, and removing classes and fields but does not allow modifications to instances. Key methods include:
- **AddClass**: Adds a new class to the project.
- **RemoveClass**: Removes a class from the project.
- **AddField**: Adds a field to a class.
- **RemoveField**: Removes a field from a class.
- **AddFieldFormatter**: Adds a field formatter to the project.
- **RemoveFieldFormatter**: Removes a field formatter from the project.
- **AddFieldChangePostProcessing**: Adds post-processing logic for field changes.
- **AddHistoryRepository**: Adds a history repository to the project.
- **RemoveHistoryRepository**: Removes a history repository from the project.
- **CloneAsRunnable**: Clones the WIP project as a `RunnableProject`.

### RunnableProject
The `RunnableProject` class represents a project that is ready for execution. It allows creating new instances but does not allow modifications to its definitions. Key methods include:
- **CreateInstance**: Creates a new instance of a class.
- **RemoveInstance**: Removes an instance by its name.
- **GetInstance**: Retrieves an instance by its name.
- **SetInstanceValue**: Sets the value of a field in an instance.
- **GetHistoryValues**: Retrieves historical values for an instance.
- **Unload**: Unloads the project and clears its instances.

### Class
The `Class` class represents a dynamic class within a project. It contains dynamic fields on which processing can be defined. Key properties and methods include:
- **Name**: The name of the class.
- **BaseClass**: The base class from which this class inherits.
- **FieldDefinitions**: A dictionary of field definitions within the class.
- **AddField**: Adds a field to the class.
- **RemoveField**: Removes a field from the class.
- **GetField**: Retrieves a field by name.
- **GetCode**: Gets the C# code for the class.
- **Clone**: Clones the class.

### Instance
The `Instance` class represents a generated instance within a project. It contains fields and their values. Key properties and methods include:
- **Fields**: A dictionary of fields within the instance.
- **GetField**: Retrieves a field by name.

### HistoryRepository
The `HistoryRepository` class is used to store the history of instance values. Key properties and methods include:
- **Name**: The name of the history repository.
- **HistoryStorageEngine**: The history storage engine used to store values.
- **UpsertRepository**: Upserts the repository in the storage engine.
- **UpsertClassTimeSerie**: Upserts a time series in the storage engine.
- **HistorizeValues**: Historizes instance values in the storage engine.

### FieldFormatter
The `FieldFormatter` class is an abstract base class for formatting fields. Key methods include:
- **ValidateAllowedType**: Validates that a type is part of the allowed types for the formatter.
- **ConvertToString**: Converts a value to a string.
- **ConvertFromString**: Converts a string to a value.

## Conclusion
The `pva.SuperV.Engine` project provides a comprehensive framework for managing and processing projects, classes, instances, and their histories. It leverages various components and dependencies to offer a robust and flexible solution for project management and data processing.
