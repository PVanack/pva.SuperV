![Ubuntu build](https://github.com/PVanack/pva.SuperV/actions/workflows/dotnet-ubuntu.yml/badge.svg?event=push)
# The pva.SuperV project
This project aims at creating a dynamic "real time" database along with processings and historization of values.
It's organized through [projects](https://github.com/PVanack/pva.SuperV/blob/master/pva.SuperV.Engine/Project.cs)
which could be either [WIP](https://github.com/PVanack/pva.SuperV/blob/master/pva.SuperV.Engine/WipProject.cs)
(i.e. with structure can be modified) (WIP stands for Work In Progress) or
[runnable](https://github.com/PVanack/pva.SuperV/blob/master/pva.SuperV.Engine/RunnableProject.cs) (i.e. with a readonly
structure, but where instamces of classes can be created).
Along fields in classes, [processing](https://github.com/PVanack/pva.SuperV/blob/master/pva.SuperV.Engine/Processing/FieldValueProcessing.cs) can be added to process value changes.

![Structure diagram](https://github.com/PVanack/pva.SuperV/blob/master/drawings/Structure.drawio.png)

The move from a *WIP* project to a *runnable* project is done by building which creates a dynamic assembly
with the [classes](https://github.com/PVanack/pva.SuperV/blob/master/pva.SuperV.Engine/Class.cs)
and [fields](https://github.com/PVanack/pva.SuperV/blob/master/pva.SuperV.Engine/FiedldDefinitions.cs) defined.
From there [instances](https://github.com/PVanack/pva.SuperV/blob/master/pva.TheSuperV.Engine/Instance.cs.cs) can be created
from a defined class. They will then have the fields and processings associated with the class they are defined against.

The solution is composed of the following projects:
- The [Engine](https://github.com/PVanack/pva.SuperV/blob/master/pva.SuperV.Engine) which holds all the intelligence
- The [REST API](https://github.com/PVanack/pva.SuperV/blob/master/pva.SuperV.Api) which allows to access the engine from clients.

The engine allows to define [fields](https://github.com/PVanack/pva.SuperV/blob/master/pva.SuperV.Engine/FiedldDefinitions.cs) of any type,
but the API needs to have concrete type not generics ones, so
the [field definition mapper](https://github.com/PVanack/pva.SuperV/blob/master/pva.SuperV.Model/FieldDefinitions/FieldDefinitionMapper.cs) and
[Field.SetValue()](https://github.com/PVanack/pva.SuperV/blob/1a9967358ab7fc57492b3b3e8be1bd9ebddf96df/pva.SuperV.Engine/Field.cs#L139) need 
to be adapted should you want to add a new field type.
