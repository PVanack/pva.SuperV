![Ubuntu build](https://github.com/PVanack/pva.SuperV/actions/workflows/dotnet-ubuntu.yml/badge.svg?event=push)
# The pva.SuperV project
This project aims at creating a dynamic "real time" database along with processings and historization of values.
It's organized through [projects](/pva.SuperV.Engine/Project.cs)
which could be either [WIP](/pva.SuperV.Engine/WipProject.cs)
(i.e. with structure can be modified) (WIP stands for Work In Progress) or
[runnable](/pva.SuperV.Engine/RunnableProject.cs) (i.e. with a readonly
structure, but where instamces of classes can be created).
Along fields in classes, [processing](/pva.SuperV.Engine/Processing/FieldValueProcessing.cs) can be added to process value changes.

![Structure diagram](/drawings/Structure.drawio.png)

The move from a *WIP* project to a *runnable* project is done by building which creates a dynamic assembly
with the [classes](/pva.SuperV.Engine/Class.cs)
and [fields](/pva.SuperV.Engine/FiedldDefinitions.cs) defined.
From there [instances](/pva.TheSuperV.Engine/Instance.cs.cs) can be created
from a defined class. They will then have the fields and processings associated with the class they are defined against.

The solution is composed of the following projects:
- The [Engine](/pva.SuperV.Engine) which holds all the intelligence
- The [REST API](/pva.SuperV.Api) which allows to access the engine from clients.

The engine allows to define [fields](/pva.SuperV.Engine/FiedldDefinitions.cs) of any type,
but the API needs to have concrete type not generics ones, so
the [field definition mapper](/pva.SuperV.Model/FieldDefinitions/FieldDefinitionMapper.cs) and
[Field.SetValue()](/pva.SuperV.Engine/Field.cs#L139) need 
to be adapted should you want to add a new field type.
