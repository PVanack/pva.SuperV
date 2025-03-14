﻿# History storage
pva.SuperV doesn't redefine a history storage backend to store the values to be historized. Instead it relies on existing solutions to store and retrieve the values.

## TDengine
The current history backend used is [TDengine](https://tdengine.com/oss/). It exists as a free version and is really fast.
It uses SQL as its syntax which is known by most people and this allows to use it from several clients.

### Supported data types
| pva.SuperV field type | TDengine data type |
|----|----|
| DateTime | TIMESTAMP |
| short | SMALLINT |
| int | INT |
| long | BIGINT |
| uint | INT UNSIGNED |
| ulong | BIGINT UNSIGNED |
| float | FLOAT |
| double | DOUBLE |
| bool | BOOL |
| string | NCHAR |
| sbyte | TINYINT |
| byte | TINYINT  |
| ushort | SMALLINT UNSIGNED |

### TDengine connection string
The connection string to be used for pva.SuperV has the following form: TDengine:host=\[*hostname*\]*;port=6030;username=\[*username*\];password=\[*password*\].
It's pretty easy to run a [docker container with the TDengine](https://hub.docker.com/r/tdengine/tdengine) already installed and running listening on port 6030.

### pva.SuperV created databases and tables
pva.SuperV does the following in TDengine:
- For each repository in projects, it creates a database with the following name: *ProjectNameRepositoryName* in **lower case**.
- For each history processing, it creates a table in the database associated with repository named like the following: *ProjectNameClassNameHistorizationProcessingName*> in **lower case**.
Its columns are named like the list of fields to be historized in **lower case** and with a tag named *instance* containing the instance name.