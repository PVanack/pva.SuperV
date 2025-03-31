Feature: Project

Background:
	Given REST application is started

@mytag
Scenario: Create project
	Given An empty WIP project "Project" is created  with "A new project" description and "TDengine" as history storage
	And History repository "HistoryRepository" is created in project "Project-WIP"
	And Enum formatter "AlarmStates" is created in project "Project-WIP"
		| Value | String    |
		| -2    | Low low   |
		| -1    | Low       |
		| 0     | OK        |
		| 1     | High      |
		| 2     | High high |
	And Enum formatter "AckStates" is created in project "Project-WIP"
		| Value | String |
		| 0     | Ack    |
		| 1     | Unack  |
	And Class "BaseClass" is created in project "Project-WIP" with the following fields
		| Name | Type | Default value | Format |
	And Class "TheClass" with base class "BaseClass" is created in project "Project-WIP" with the following fields
		| Name          | Type   | Default value | Format      |
		| Value         | double | 50            |             |
		| HighHighLimit | double | 100           |             |
		| HighLimit     | double | 75            |             |
		| LowLimit      | double | 25            |             |
		| LowLowLimit   | double | 0             |             |
		| AlarmState    | int    | 0             | AlarmStates |
		| AckState      | int    | 0             | AckStates   |
	And Alarm state processing "AlarmState" is created on field "Value" of class "TheClass" of project "Project-WIP"
		| HighHigh limit field | High limit field | Low limit field | LowLow limit field | Deadband field | AlarmState field | AckState field |
		| HighHighLimit        | HighLimit        | LowLimit        | LowLowLimit        |                | AlarmState       | AckState       |
	And Historization processing "ValueHistorization" is created on field "Value" of class "TheClass" of project "Project-WIP"
		| History repository | Timestamp field | Field to historize |
		| HistoryRepository  |                 |                    |
		|                    |                 | Value              |
		|                    |                 | AlarmState         |
		|                    |                 | AckState           |

	And Class "AllFieldsClass" is created in project "Project-WIP" with the following fields
		| Name           | Type     | Default value        | Format |
		| Bool           | bool     | true                 |        |
		| DateTime       | DateTime | 2025-03-01T00:00:00Z |        |
		| Double         | double   | 12.3                 |        |
		| Float          | float    | 3.21                 |        |
		| Int            | int      | 134567               |        |
		| Long           | long     | 9876543              |        |
		| Short          | short    | 32767                |        |
		| String         | string   | Hi from SuperV!      |        |
		| TimeSpan       | TimeSpan | 01:02:59             |        |
		| Uint           | uint     | 123456               |        |
		| Ulong          | ulong    | 98123456             |        |
		| Ushort         | ushort   | 32767                |        |
		| HistoryTrigger | int      | 0                    |        |

	And Historization processing "ValueHistorization" is created on field "HistoryTrigger" of class "AllFieldsClass" of project "Project-WIP"
		| History repository | Timestamp field | Field to historize |
		| HistoryRepository  |                 |                    |
		|                    |                 | Bool               |
		|                    |                 | Double             |
		|                    |                 | Float              |
		|                    |                 | Int                |
		|                    |                 | Long               |
		|                    |                 | Short              |
		|                    |                 | String             |
		|                    |                 | TimeSpan           |
		|                    |                 | Uint               |
		|                    |                 | Ulong              |
		|                    |                 | Ushort             |
		|                    |                 | HistoryTrigger     |

	And Runnable project is built from WIP project "Project-WIP"
	And Instance "AnInstance" is created with class "TheClass" in project "Project"
		| Name  | Type   | Value |
		| Value | double | 50    |
	And Instance "AnInstance" fields values are updated in project "Project"
		| Name          | Type   | Value | Quality | Timestamp |
		| Value         | double | 50    |         |           |
		| HighHighLimit | double | 99    |         |           |
		| LowLowLimit   | double | 1     |         |           |
		| AckState      | string | Unack |         |           |

	Then Instance "AnInstance" fields have expected values in project "Project"
		| Name          | Type   | Value | Quality | Timestamp | Formatted value |
		| Value         | double | 50    |         |           |                 |
		| HighHighLimit | double | 99    |         |           |                 |
		| LowLowLimit   | double | 1     |         |           |                 |
		| AckState      | int    | 1     |         |           | Unack           |

	Given Instance "AllFieldsInstance" is created with class "AllFieldsClass" in project "Project"
		| Name | Type | Value |

	And Instance "AllFieldsInstance" fields values are updated in project "Project"
		| Name     | Type     | Value                |
		| Bool     | bool     | true                 |
		| DateTime | DateTime | 2025-03-01T00:00:00Z |
		| Double   | double   | 12.3                 |
		| Float    | float    | 3.21                 |
		| Int      | int      | 134567               |
		| Long     | long     | 9876543              |
		| Short    | short    | 32767                |
		| String   | string   | Hi from SuperV!      |
		| TimeSpan | TimeSpan | 01:02:59             |
		| Uint     | uint     | 123456               |
		| Ulong    | ulong    | 98123456             |
		| Ushort   | ushort   | 32767                |

	Then Instance "AllFieldsInstance" fields have expected values in project "Project"
		| Name     | Type     | Value                |
		| Bool     | bool     | true                 |
		| DateTime | DateTime | 2025-03-01T00:00:00Z |
		| Double   | double   | 12.3                 |
		| Float    | float    | 3.21                 |
		| Int      | int      | 134567               |
		| Long     | long     | 9876543              |
		| Short    | short    | 32767                |
		| String   | string   | Hi from SuperV!      |
		| TimeSpan | TimeSpan | 01:02:59             |
		| Uint     | uint     | 123456               |
		| Ulong    | ulong    | 98123456             |
		| Ushort   | ushort   | 32767                |

	Given Instance "AllFieldsInstance" fields values are updated in project "Project"
		| Name           | Type     | Value                | Quality | Timestamp            |
		| Bool           | bool     | true                 | Good    | 2025-03-01T00:00:00Z |
		| DateTime       | DateTime | 2025-03-01T00:00:00Z | Good    | 2025-03-01T00:00:00Z |
		| Double         | double   | 12.3                 | Good    | 2025-03-01T00:00:00Z |
		| Float          | float    | 3.21                 | Good    | 2025-03-01T00:00:00Z |
		| Int            | int      | 134567               | Good    | 2025-03-01T00:00:00Z |
		| Long           | long     | 9876543              | Good    | 2025-03-01T00:00:00Z |
		| Short          | short    | 32767                | Good    | 2025-03-01T00:00:00Z |
		| String         | string   | Hi from SuperV!      | Good    | 2025-03-01T00:00:00Z |
		| TimeSpan       | TimeSpan | 01:02:59             | Good    | 2025-03-01T00:00:00Z |
		| Uint           | uint     | 123456               | Good    | 2025-03-01T00:00:00Z |
		| Ulong          | ulong    | 98123456             | Good    | 2025-03-01T00:00:00Z |
		| Ushort         | ushort   | 32767                | Good    | 2025-03-01T00:00:00Z |
		| HistoryTrigger | int      | 0                    | Good    | 2025-03-01T00:00:00Z |

	And Instance "AllFieldsInstance" fields values are updated in project "Project"
		| Name           | Type     | Value                | Quality | Timestamp            |
		| Bool           | bool     | false                | Good    | 2025-03-01T00:59:59Z |
		| DateTime       | DateTime | 2025-03-01T00:59:59Z | Good    | 2025-03-01T00:59:59Z |
		| Double         | double   | 3.12                 | Good    | 2025-03-01T00:59:59Z |
		| Float          | float    | 21.3                 | Good    | 2025-03-01T00:59:59Z |
		| Int            | int      | 5654321              | Good    | 2025-03-01T00:59:59Z |
		| Long           | long     | 3456789              | Good    | 2025-03-01T00:59:59Z |
		| Short          | short    | -32767               | Good    | 2025-03-01T00:59:59Z |
		| String         | string   | Bye from SuperV!     | Good    | 2025-03-01T00:59:59Z |
		| TimeSpan       | TimeSpan | 02:03:01             | Good    | 2025-03-01T00:59:59Z |
		| Uint           | uint     | 654321               | Good    | 2025-03-01T00:59:59Z |
		| Ulong          | ulong    | 654789               | Good    | 2025-03-01T00:59:59Z |
		| Ushort         | ushort   | 12768                | Good    | 2025-03-01T00:59:59Z |
		| HistoryTrigger | int      | 1                    | Good    | 2025-03-01T00:59:59Z |

	Then Querying raw history values of instance "AllFieldsInstance" in project "Project" between "2025-03-01T00:00:00Z" and "2025-03-01T01:00:00Z" returns fields history values
		| Ts                   | Quality | Bool,bool | Double,double | Float,float | Int,int | Long,long | Short,short | String,string    | TimeSpan,TimeSpan | Uint,uint | Ulong,ulong | Ushort,ushort | HistoryTrigger,int |
		| 2025-03-01T00:00:00Z | Good    | true      | 12.3          | 3.21        | 134567  | 9876543   | 32767       | Hi from SuperV!  | 01:02:59          | 123456    | 98123456    | 32767         | 0                  |
		| 2025-03-01T00:59:59Z | Good    | false     | 3.12          | 21.3        | 5654321 | 3456789   | -32767      | Bye from SuperV! | 02:03:01          | 654321    | 654789      | 12768         | 1                  |

	And Querying history values of instance "AllFieldsInstance" in project "Project" between "2025-03-01T00:00:00Z" and "2025-03-01T01:00:00Z" returns fields history values
		| Ts                   | Quality | Bool,bool | Double,double | Float,float | Int,int | Long,long | Short,short | String,string    | TimeSpan,TimeSpan | Uint,uint | Ulong,ulong | Ushort,ushort | HistoryTrigger,int |
		| 2025-03-01T00:00:00Z | Good    | true      | 12.3          | 3.21        | 134567  | 9876543   | 32767       | Hi from SuperV!  | 01:02:59          | 123456    | 98123456    | 32767         | 0                  |
		| 2025-03-01T00:59:59Z | Good    | false     | 3.12          | 21.3        | 5654321 | 3456789   | -32767      | Bye from SuperV! | 02:03:01          | 654321    | 654789      | 12768         | 1                  |

	Given Instance "AllFieldsInstance" fields values are updated in project "Project"
		| Name           | Type     | Value                | Quality | Timestamp            |
		| Bool           | bool     | true                 | Good    | 2025-03-01T00:00:00Z |
		| DateTime       | DateTime | 2025-03-01T00:00:00Z | Good    | 2025-03-01T00:00:00Z |
		| Double         | double   | 5                    | Good    | 2025-03-01T00:00:00Z |
		| Float          | float    | 3.21                 | Good    | 2025-03-01T00:00:00Z |
		| Int            | int      | 134567               | Good    | 2025-03-01T00:00:00Z |
		| Long           | long     | 9876543              | Good    | 2025-03-01T00:00:00Z |
		| Short          | short    | 32767                | Good    | 2025-03-01T00:00:00Z |
		| String         | string   | Hi from SuperV!      | Good    | 2025-03-01T00:00:00Z |
		| TimeSpan       | TimeSpan | 01:02:59             | Good    | 2025-03-01T00:00:00Z |
		| Uint           | uint     | 123456               | Good    | 2025-03-01T00:00:00Z |
		| Ulong          | ulong    | 98123456             | Good    | 2025-03-01T00:00:00Z |
		| Ushort         | ushort   | 32767                | Good    | 2025-03-01T00:00:00Z |
		| HistoryTrigger | int      | 0                    | Good    | 2025-03-01T00:00:00Z |

	And Instance "AllFieldsInstance" fields values are updated in project "Project"
		| Name           | Type     | Value                | Quality | Timestamp            |
		| Bool           | bool     | false                | Good    | 2025-03-01T00:59:59Z |
		| DateTime       | DateTime | 2025-03-01T00:59:59Z | Good    | 2025-03-01T00:59:59Z |
		| Double         | double   | 10                   | Good    | 2025-03-01T00:59:59Z |
		| Float          | float    | 21.3                 | Good    | 2025-03-01T00:59:59Z |
		| Int            | int      | 5654321              | Good    | 2025-03-01T00:59:59Z |
		| Long           | long     | 3456789              | Good    | 2025-03-01T00:59:59Z |
		| Short          | short    | -32767               | Good    | 2025-03-01T00:59:59Z |
		| String         | string   | Bye from SuperV!     | Good    | 2025-03-01T00:59:59Z |
		| TimeSpan       | TimeSpan | 02:03:01             | Good    | 2025-03-01T00:59:59Z |
		| Uint           | uint     | 654321               | Good    | 2025-03-01T00:59:59Z |
		| Ulong          | ulong    | 654789               | Good    | 2025-03-01T00:59:59Z |
		| Ushort         | ushort   | 12768                | Good    | 2025-03-01T00:59:59Z |
		| HistoryTrigger | int      | 1                    | Good    | 2025-03-01T00:59:59Z |

	Then Querying raw history statistics of instance "AllFieldsInstance" in project "Project" between "2025-03-01T00:00:00Z" and "2025-03-01T01:00:00Z" with "01:00:00" interval returns fields statistic values
		| StartTs              | EndTs                | Quality | Double,double,TWA | Double,double,AVG | HistoryTrigger,int,MIN |
		| 2025-03-01T00:00:00Z | 2025-03-01T01:00:00Z | Good    | 7.5               | 7.5               | 0                      |

	And Querying history statistics of instance "AllFieldsInstance" in project "Project" between "2025-03-01T00:00:00Z" and "2025-03-01T01:00:00Z" with "01:00:00" interval returns fields statistic values
		| StartTs              | EndTs                | Quality | Double,double,TWA | Double,double,MIN |  Double,double,AVG | Double,double,MAX | Double,long,COUNT   | HistoryTrigger,int,MIN |
		| 2025-03-01T00:00:00Z | 2025-03-01T01:00:00Z | Good    | 7.5               | 5.0               |  7.5               | 10.0              | 2                   | 0                      |

#	And Querying raw history statistics of instance "AllFieldsInstance" in project "Project" between "2025-03-01T00:00:00Z" and "2025-03-01T01:00:00Z" with "01:00:00" interval returns fields history values
#		| StartTs              | EndTs                | Quality | Float,float,MAX | Int,int,MAX | Long,long,MAX | Short,short,MAX | String,string,MAX | TimeSpan,TimeSpan,MAX | Uint,uint,MAX | Ulong,ulong,MAX | Ushort,ushort,MAX | HistoryTrigger,int,MAX |
#		| 2025-03-01T00:00:00Z | 2025-03-01T00:00:00Z | Good    | 3.21            | 134567      | 9876543       | 32767           | Hi from SuperV!   | 01:02:59              | 123456        | 98123456        | 32767             | 1                      |
#		| 2025-03-01T00:59:59Z | 2025-03-01T00:00:00Z | Good    | 21.3            | 5654321     | 3456789       | -32767          | Bye from SuperV!  | 02:03:01              | 654321        | 654789          | 12768             | 1                      |

	Then TDengine is stopped if running
