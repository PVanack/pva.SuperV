Feature: Project

Background:
	Given REST application is started

@mytag
Scenario: Create project
	Given An empty WIP project "Project" is created  with "A new project" description and "TDengine" as history storage
	And History repository "HistoryRepository" is created in project "Project-WIP"
	And Enum formatter "AlarmStates" is created in project "Project-WIP"
		| Value | String    |
		|    -2 | Low low   |
		|    -1 | Low       |
		|     0 | OK        |
		|     1 | High      |
		|     2 | High high |
	And Enum formatter "AckStates" is created in project "Project-WIP"
		| Value | String |
		|     0 | Ack    |
		|     1 | Unack  |
	And Class "BaseClass" is created in project "Project-WIP" with the following fields
		| Name | Type | Default value | Format | Topic |
	And Class "TheClass" with base class "BaseClass" is created in project "Project-WIP" with the following fields
		| Name                | Type   | Default value | Format      | Topic |
		| DoubleValue         | double |            50 |             |       |
		| DoubleHighHighLimit | double |           100 |             |       |
		| DoubleHighLimit     | double |            75 |             |       |
		| DoubleLowLimit      | double |            25 |             |       |
		| DoubleLowLowLimit   | double |             0 |             |       |
		| DoubleAlarmState    | int    |             0 | AlarmStates |       |
		| DoubleAckState      | int    |             0 | AckStates   |       |
		| FloatValue          | float  |            50 |             |       |
		| FloatHighHighLimit  | float  |           100 |             |       |
		| FloatHighLimit      | float  |            75 |             |       |
		| FloatLowLimit       | float  |            25 |             |       |
		| FloatLowLowLimit    | float  |             0 |             |       |
		| FloatAlarmState     | int    |             0 | AlarmStates |       |
		| FloatAckState       | int    |             0 | AckStates   |       |
		| IntValue            | int    |            50 |             |       |
		| IntHighHighLimit    | int    |           100 |             |       |
		| IntHighLimit        | int    |            75 |             |       |
		| IntLowLimit         | int    |            25 |             |       |
		| IntLowLowLimit      | int    |             0 |             |       |
		| IntAlarmState       | int    |             0 | AlarmStates |       |
		| IntAckState         | int    |             0 | AckStates   |       |
		| LongValue           | long   |            50 |             |       |
		| LongHighHighLimit   | long   |           100 |             |       |
		| LongHighLimit       | long   |            75 |             |       |
		| LongLowLimit        | long   |            25 |             |       |
		| LongLowLowLimit     | long   |             0 |             |       |
		| LongAlarmState      | int    |             0 | AlarmStates |       |
		| LongAckState        | int    |             0 | AckStates   |       |
		| ShortValue          | short  |            50 |             |       |
		| ShortHighHighLimit  | short  |           100 |             |       |
		| ShortHighLimit      | short  |            75 |             |       |
		| ShortLowLimit       | short  |            25 |             |       |
		| ShortLowLowLimit    | short  |             0 |             |       |
		| ShortAlarmState     | int    |             0 | AlarmStates |       |
		| ShortAckState       | int    |             0 | AckStates   |       |
		| UIntValue           | uint   |            50 |             |       |
		| UIntHighHighLimit   | uint   |           100 |             |       |
		| UIntHighLimit       | uint   |            75 |             |       |
		| UIntLowLimit        | uint   |            25 |             |       |
		| UIntLowLowLimit     | uint   |             0 |             |       |
		| UIntAlarmState      | int    |             0 | AlarmStates |       |
		| UIntAckState        | int    |             0 | AckStates   |       |
		| ULongValue          | ulong  |            50 |             |       |
		| ULongHighHighLimit  | ulong  |           100 |             |       |
		| ULongHighLimit      | ulong  |            75 |             |       |
		| ULongLowLimit       | ulong  |            25 |             |       |
		| ULongLowLowLimit    | ulong  |             0 |             |       |
		| ULongAlarmState     | int    |             0 | AlarmStates |       |
		| ULongAckState       | int    |             0 | AckStates   |       |
		| UShortValue         | ushort |            50 |             |       |
		| UShortHighHighLimit | ushort |           100 |             |       |
		| UShortHighLimit     | ushort |            75 |             |       |
		| UShortLowLimit      | ushort |            25 |             |       |
		| UShortLowLowLimit   | ushort |             0 |             |       |
		| UShortAlarmState    | int    |             0 | AlarmStates |       |
		| UShortAckState      | int    |             0 | AckStates   |       |
	And Alarm state processing "DoubleAlarmState" is created on field "DoubleValue" of class "TheClass" of project "Project-WIP"
		| HighHigh limit field | High limit field | Low limit field | LowLow limit field | Deadband field | AlarmState field | AckState field |
		| DoubleHighHighLimit  | DoubleHighLimit  | DoubleLowLimit  | DoubleLowLowLimit  |                | DoubleAlarmState | DoubleAckState |
	And Alarm state processing "FloatAlarmState" is created on field "FloatValue" of class "TheClass" of project "Project-WIP"
		| HighHigh limit field | High limit field | Low limit field | LowLow limit field | Deadband field | AlarmState field | AckState field |
		| FloatHighHighLimit   | FloatHighLimit   | FloatLowLimit   | FloatLowLowLimit   |                | FloatAlarmState  | FloatAckState  |
	And Alarm state processing "IntAlarmState" is created on field "IntValue" of class "TheClass" of project "Project-WIP"
		| HighHigh limit field | High limit field | Low limit field | LowLow limit field | Deadband field | AlarmState field | AckState field |
		| IntHighHighLimit     | IntHighLimit     | IntLowLimit     | IntLowLowLimit     |                | IntAlarmState    | IntAckState    |
	And Alarm state processing "ShortAlarmState" is created on field "ShortValue" of class "TheClass" of project "Project-WIP"
		| HighHigh limit field | High limit field | Low limit field | LowLow limit field | Deadband field | AlarmState field | AckState field |
		| ShortHighHighLimit   | ShortHighLimit   | ShortLowLimit   | ShortLowLowLimit   |                | ShortAlarmState  | ShortAckState  |
	And Alarm state processing "LongAlarmState" is created on field "LongValue" of class "TheClass" of project "Project-WIP"
		| HighHigh limit field | High limit field | Low limit field | LowLow limit field | Deadband field | AlarmState field | AckState field |
		| LongHighHighLimit    | LongHighLimit    | LongLowLimit    | LongLowLowLimit    |                | LongAlarmState   | LongAckState   |
	And Alarm state processing "UIntAlarmState" is created on field "UIntValue" of class "TheClass" of project "Project-WIP"
		| HighHigh limit field | High limit field | Low limit field | LowLow limit field | Deadband field | AlarmState field | AckState field |
		| UIntHighHighLimit    | UIntHighLimit    | UIntLowLimit    | UIntLowLowLimit    |                | UIntAlarmState   | UIntAckState   |
	And Alarm state processing "UShortAlarmState" is created on field "UShortValue" of class "TheClass" of project "Project-WIP"
		| HighHigh limit field | High limit field | Low limit field | LowLow limit field | Deadband field | AlarmState field | AckState field |
		| UShortHighHighLimit  | UShortHighLimit  | UShortLowLimit  | UShortLowLowLimit  |                | UShortAlarmState | UShortAckState |
	And Alarm state processing "ULongAlarmState" is created on field "ULongValue" of class "TheClass" of project "Project-WIP"
		| HighHigh limit field | High limit field | Low limit field | LowLow limit field | Deadband field | AlarmState field | AckState field |
		| ULongHighHighLimit   | ULongHighLimit   | ULongLowLimit   | ULongLowLowLimit   |                | ULongAlarmState  | ULongAckState  |
	And Historization processing "DoubleValueHistorization" is created on field "DoubleValue" of class "TheClass" of project "Project-WIP"
		| History repository | Timestamp field | Field to historize |
		| HistoryRepository  |                 |                    |
		|                    |                 | DoubleValue        |
		|                    |                 | DoubleAlarmState   |
		|                    |                 | DoubleAckState     |
	And Historization processing "FloatValueHistorization" is created on field "FloatValue" of class "TheClass" of project "Project-WIP"
		| History repository | Timestamp field | Field to historize |
		| HistoryRepository  |                 |                    |
		|                    |                 | FloatValue         |
	And Historization processing "IntValueHistorization" is created on field "IntValue" of class "TheClass" of project "Project-WIP"
		| History repository | Timestamp field | Field to historize |
		| HistoryRepository  |                 |                    |
		|                    |                 | IntValue           |
	And Historization processing "LongValueHistorization" is created on field "LongValue" of class "TheClass" of project "Project-WIP"
		| History repository | Timestamp field | Field to historize |
		| HistoryRepository  |                 |                    |
		|                    |                 | LongValue          |
	And Historization processing "ShortValueHistorization" is created on field "ShortValue" of class "TheClass" of project "Project-WIP"
		| History repository | Timestamp field | Field to historize |
		| HistoryRepository  |                 |                    |
		|                    |                 | ShortValue         |

	And Historization processing "UIntValueHistorization" is created on field "UIntValue" of class "TheClass" of project "Project-WIP"
		| History repository | Timestamp field | Field to historize |
		| HistoryRepository  |                 |                    |
		|                    |                 | UIntValue          |
	And Historization processing "ULongValueHistorization" is created on field "ULongValue" of class "TheClass" of project "Project-WIP"
		| History repository | Timestamp field | Field to historize |
		| HistoryRepository  |                 |                    |
		|                    |                 | ULongValue         |
	And Historization processing "UShortValueHistorization" is created on field "UShortValue" of class "TheClass" of project "Project-WIP"
		| History repository | Timestamp field | Field to historize |
		| HistoryRepository  |                 |                    |
		|                    |                 | UShortValue        |

	And Class "AllFieldsClass" is created in project "Project-WIP" with the following fields
		| Name           | Type     | Default value        | Format | Topic |
		| Bool           | bool     | true                 |        |       |
		| DateTime       | DateTime | 2025-03-01T00:00:00Z |        |       |
		| Double         | double   |                 12.3 |        |       |
		| Float          | float    |                 3.21 |        |       |
		| Int            | int      |               134567 |        |       |
		| Long           | long     |              9876543 |        |       |
		| Short          | short    |                32767 |        |       |
		| String         | string   | Hi from SuperV!      |        |       |
		| TimeSpan       | TimeSpan | 01:02:59             |        |       |
		| Uint           | uint     |               123456 |        |       |
		| Ulong          | ulong    |             98123456 |        |       |
		| Ushort         | ushort   |                32767 |        |       |
		| HistoryTrigger | int      |                    0 |        |       |
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

	Then Getting classes of project "Project" returns the following classes
		| Name           | Base class |
		| BaseClass      |            |
		| TheClass       | BaseClass  |
		| AllFieldsClass |            |
	And Searching "Class" classes of project "Project" returns the following classes
		| Name           | Base class |
		| BaseClass      |            |
		| TheClass       | BaseClass  |
		| AllFieldsClass |            |
	And Searching "Base" classes of project "Project" returns the following classes
		| Name      | Base class |
		| BaseClass |            |
	And Getting fields of class "TheClass" of project "Project" returns the following fields
		| Name                | Type   | Default value | Format      | Topic |
		| DoubleValue         | double |            50 |             |       |
		| DoubleHighHighLimit | double |           100 |             |       |
		| DoubleHighLimit     | double |            75 |             |       |
		| DoubleLowLimit      | double |            25 |             |       |
		| DoubleLowLowLimit   | double |             0 |             |       |
		| DoubleAlarmState    | int    |             0 | AlarmStates |       |
		| DoubleAckState      | int    |             0 | AckStates   |       |
		| FloatValue          | float  |            50 |             |       |
		| FloatHighHighLimit  | float  |           100 |             |       |
		| FloatHighLimit      | float  |            75 |             |       |
		| FloatLowLimit       | float  |            25 |             |       |
		| FloatLowLowLimit    | float  |             0 |             |       |
		| FloatAlarmState     | int    |             0 | AlarmStates |       |
		| FloatAckState       | int    |             0 | AckStates   |       |
		| IntValue            | int    |            50 |             |       |
		| IntHighHighLimit    | int    |           100 |             |       |
		| IntHighLimit        | int    |            75 |             |       |
		| IntLowLimit         | int    |            25 |             |       |
		| IntLowLowLimit      | int    |             0 |             |       |
		| IntAlarmState       | int    |             0 | AlarmStates |       |
		| IntAckState         | int    |             0 | AckStates   |       |
		| LongValue           | long   |            50 |             |       |
		| LongHighHighLimit   | long   |           100 |             |       |
		| LongHighLimit       | long   |            75 |             |       |
		| LongLowLimit        | long   |            25 |             |       |
		| LongLowLowLimit     | long   |             0 |             |       |
		| LongAlarmState      | int    |             0 | AlarmStates |       |
		| LongAckState        | int    |             0 | AckStates   |       |
		| ShortValue          | short  |            50 |             |       |
		| ShortHighHighLimit  | short  |           100 |             |       |
		| ShortHighLimit      | short  |            75 |             |       |
		| ShortLowLimit       | short  |            25 |             |       |
		| ShortLowLowLimit    | short  |             0 |             |       |
		| ShortAlarmState     | int    |             0 | AlarmStates |       |
		| ShortAckState       | int    |             0 | AckStates   |       |
		| UIntValue           | uint   |            50 |             |       |
		| UIntHighHighLimit   | uint   |           100 |             |       |
		| UIntHighLimit       | uint   |            75 |             |       |
		| UIntLowLimit        | uint   |            25 |             |       |
		| UIntLowLowLimit     | uint   |             0 |             |       |
		| UIntAlarmState      | int    |             0 | AlarmStates |       |
		| UIntAckState        | int    |             0 | AckStates   |       |
		| ULongValue          | ulong  |            50 |             |       |
		| ULongHighHighLimit  | ulong  |           100 |             |       |
		| ULongHighLimit      | ulong  |            75 |             |       |
		| ULongLowLimit       | ulong  |            25 |             |       |
		| ULongLowLowLimit    | ulong  |             0 |             |       |
		| ULongAlarmState     | int    |             0 | AlarmStates |       |
		| ULongAckState       | int    |             0 | AckStates   |       |
		| UShortValue         | ushort |            50 |             |       |
		| UShortHighHighLimit | ushort |           100 |             |       |
		| UShortHighLimit     | ushort |            75 |             |       |
		| UShortLowLimit      | ushort |            25 |             |       |
		| UShortLowLowLimit   | ushort |             0 |             |       |
		| UShortAlarmState    | int    |             0 | AlarmStates |       |
		| UShortAckState      | int    |             0 | AckStates   |       |
	And Searching "Double" fields of class "TheClass" of project "Project" returns the following fields
		| Name                | Type   | Default value | Format      | Topic |
		| DoubleValue         | double |            50 |             |       |
		| DoubleHighHighLimit | double |           100 |             |       |
		| DoubleHighLimit     | double |            75 |             |       |
		| DoubleLowLimit      | double |            25 |             |       |
		| DoubleLowLowLimit   | double |             0 |             |       |
		| DoubleAlarmState    | int    |             0 | AlarmStates |       |
		| DoubleAckState      | int    |             0 | AckStates   |       |

	Given Instance "AnInstance" is created with class "TheClass" in project "Project"
		| Name        | Type   | Value |
		| DoubleValue | double |    25 |
	When Instance "AnInstance" fields values are updated in project "Project"
		| Name                | Type   | Value | Quality | Timestamp |
		| DoubleValue         | double |    50 |         |           |
		| DoubleHighHighLimit | double |    99 |         |           |
		| DoubleHighLimit     | double |    75 |         |           |
		| DoubleLowLimit      | double |    15 |         |           |
		| DoubleLowLowLimit   | double |     1 |         |           |
		| DoubleAckState      | string | Unack |         |           |

	Then Instance "AnInstance" fields have expected values in project "Project"
		| Name                | Type   | Value | Quality | Timestamp | Formatted value |
		| DoubleValue         | double |    50 |         |           |                 |
		| DoubleHighHighLimit | double |    99 |         |           |                 |
		| DoubleLowLowLimit   | double |     1 |         |           |                 |
		| DoubleAlarmState    | int    |     0 |         |           | OK              |
		| DoubleAckState      | int    |     1 |         |           | Unack           |

	When Instance "AnInstance" fields values are updated in project "Project"
		| Name           | Type   | Value | Quality | Timestamp |
		| DoubleAckState | string | Ack   |         |           |
		| DoubleValue    | double |    79 |         |           |
	Then Instance "AnInstance" fields have expected values in project "Project"
		| Name             | Type   | Value | Quality | Timestamp | Formatted value |
		| DoubleValue      | double |    79 |         |           |                 |
		| DoubleAlarmState | int    |     1 |         |           | High            |
		| DoubleAckState   | int    |     1 |         |           | Unack           |

	When Instance "AnInstance" fields values are updated in project "Project"
		| Name           | Type   | Value | Quality | Timestamp |
		| DoubleAckState | string | Ack   |         |           |
		| DoubleValue    | double |   110 |         |           |
	Then Instance "AnInstance" fields have expected values in project "Project"
		| Name             | Type   | Value | Quality | Timestamp | Formatted value |
		| DoubleValue      | double |   110 |         |           |                 |
		| DoubleAlarmState | int    |     2 |         |           | High high       |
		| DoubleAckState   | int    |     1 |         |           | Unack           |

	When Instance "AnInstance" fields values are updated in project "Project"
		| Name           | Type   | Value | Quality | Timestamp |
		| DoubleAckState | string | Ack   |         |           |
		| DoubleValue    | double |    10 |         |           |
	Then Instance "AnInstance" fields have expected values in project "Project"
		| Name             | Type   | Value | Quality | Timestamp | Formatted value |
		| DoubleValue      | double |    10 |         |           |                 |
		| DoubleAlarmState | int    |    -1 |         |           | Low             |
		| DoubleAckState   | int    |     1 |         |           | Unack           |

	When Instance "AnInstance" fields values are updated in project "Project"
		| Name           | Type   | Value | Quality | Timestamp |
		| DoubleAckState | string | Ack   |         |           |
		| DoubleValue    | double |     0 |         |           |
	Then Instance "AnInstance" fields have expected values in project "Project"
		| Name             | Type   | Value | Quality | Timestamp | Formatted value |
		| DoubleValue      | double |     0 |         |           |                 |
		| DoubleAlarmState | int    |    -2 |         |           | Low low         |
		| DoubleAckState   | int    |     1 |         |           | Unack           |

	Given Instance "AllFieldsInstance" is created with class "AllFieldsClass" in project "Project"
		| Name | Type | Value |

	When Instance "AllFieldsInstance" fields values are updated in project "Project"
		| Name     | Type     | Value                |
		| Bool     | bool     | true                 |
		| DateTime | DateTime | 2025-03-01T00:00:00Z |
		| Double   | double   |                 12.3 |
		| Float    | float    |                 3.21 |
		| Int      | int      |               134567 |
		| Long     | long     |              9876543 |
		| Short    | short    |                32767 |
		| String   | string   | Hi from SuperV!      |
		| TimeSpan | TimeSpan | 01:02:59             |
		| Uint     | uint     |               123456 |
		| Ulong    | ulong    |             98123456 |
		| Ushort   | ushort   |                32767 |

	Then Instance "AllFieldsInstance" fields have expected values in project "Project"
		| Name     | Type     | Value                |
		| Bool     | bool     | true                 |
		| DateTime | DateTime | 2025-03-01T00:00:00Z |
		| Double   | double   |                 12.3 |
		| Float    | float    |                 3.21 |
		| Int      | int      |               134567 |
		| Long     | long     |              9876543 |
		| Short    | short    |                32767 |
		| String   | string   | Hi from SuperV!      |
		| TimeSpan | TimeSpan | 01:02:59             |
		| Uint     | uint     |               123456 |
		| Ulong    | ulong    |             98123456 |
		| Ushort   | ushort   |                32767 |

	When Instance "AllFieldsInstance" fields values are updated in project "Project"
		| Name           | Type     | Value                | Quality | Timestamp            |
		| Bool           | bool     | true                 | Good    | 2025-03-01T00:00:00Z |
		| DateTime       | DateTime | 2025-03-01T00:00:00Z | Good    | 2025-03-01T00:00:00Z |
		| Double         | double   |                 12.3 | Good    | 2025-03-01T00:00:00Z |
		| Float          | float    |                 3.21 | Good    | 2025-03-01T00:00:00Z |
		| Int            | int      |               134567 | Good    | 2025-03-01T00:00:00Z |
		| Long           | long     |              9876543 | Good    | 2025-03-01T00:00:00Z |
		| Short          | short    |                32767 | Good    | 2025-03-01T00:00:00Z |
		| String         | string   | Hi from SuperV!      | Good    | 2025-03-01T00:00:00Z |
		| TimeSpan       | TimeSpan | 01:02:59             | Good    | 2025-03-01T00:00:00Z |
		| Uint           | uint     |               123456 | Good    | 2025-03-01T00:00:00Z |
		| Ulong          | ulong    |             98123456 | Good    | 2025-03-01T00:00:00Z |
		| Ushort         | ushort   |                32767 | Good    | 2025-03-01T00:00:00Z |
		| HistoryTrigger | int      |                    0 | Good    | 2025-03-01T00:00:00Z |

	And Instance "AllFieldsInstance" fields values are updated in project "Project"
		| Name           | Type     | Value                | Quality | Timestamp            |
		| Bool           | bool     | false                | Good    | 2025-03-01T00:59:59Z |
		| DateTime       | DateTime | 2025-03-01T00:59:59Z | Good    | 2025-03-01T00:59:59Z |
		| Double         | double   |                 3.12 | Good    | 2025-03-01T00:59:59Z |
		| Float          | float    |                 21.3 | Good    | 2025-03-01T00:59:59Z |
		| Int            | int      |              5654321 | Good    | 2025-03-01T00:59:59Z |
		| Long           | long     |              3456789 | Good    | 2025-03-01T00:59:59Z |
		| Short          | short    |               -32767 | Good    | 2025-03-01T00:59:59Z |
		| String         | string   | Bye from SuperV!     | Good    | 2025-03-01T00:59:59Z |
		| TimeSpan       | TimeSpan | 02:03:01             | Good    | 2025-03-01T00:59:59Z |
		| Uint           | uint     |               654321 | Good    | 2025-03-01T00:59:59Z |
		| Ulong          | ulong    |               654789 | Good    | 2025-03-01T00:59:59Z |
		| Ushort         | ushort   |                12768 | Good    | 2025-03-01T00:59:59Z |
		| HistoryTrigger | int      |                    1 | Good    | 2025-03-01T00:59:59Z |

	Then Querying raw history values of instance "AllFieldsInstance" in project "Project" between "2025-03-01T00:00:00Z" and "2025-03-01T01:00:00Z" returns fields history values
		| Ts                   | Quality | Bool,bool | Double,double | Float,float | Int,int | Long,long | Short,short | String,string    | TimeSpan,TimeSpan | Uint,uint | Ulong,ulong | Ushort,ushort | HistoryTrigger,int |
		| 2025-03-01T00:00:00Z | Good    | true      |          12.3 |        3.21 |  134567 |   9876543 |       32767 | Hi from SuperV!  | 01:02:59          |    123456 |    98123456 |         32767 |                  0 |
		| 2025-03-01T00:59:59Z | Good    | false     |          3.12 |        21.3 | 5654321 |   3456789 |      -32767 | Bye from SuperV! | 02:03:01          |    654321 |      654789 |         12768 |                  1 |

	And Querying history values of instance "AllFieldsInstance" in project "Project" between "2025-03-01T00:00:00Z" and "2025-03-01T01:00:00Z" returns fields history values
		| Ts                   | Quality | Bool,bool | Double,double | Float,float | Int,int | Long,long | Short,short | String,string    | TimeSpan,TimeSpan | Uint,uint | Ulong,ulong | Ushort,ushort | HistoryTrigger,int |
		| 2025-03-01T00:00:00Z | Good    | true      |          12.3 |        3.21 |  134567 |   9876543 |       32767 | Hi from SuperV!  | 01:02:59          |    123456 |    98123456 |         32767 |                  0 |
		| 2025-03-01T00:59:59Z | Good    | false     |          3.12 |        21.3 | 5654321 |   3456789 |      -32767 | Bye from SuperV! | 02:03:01          |    654321 |      654789 |         12768 |                  1 |

	When Instance "AllFieldsInstance" fields values are updated in project "Project"
		| Name           | Type     | Value                | Quality | Timestamp            |
		| Bool           | bool     | true                 | Good    | 2025-03-01T00:00:00Z |
		| DateTime       | DateTime | 2025-03-01T00:00:00Z | Good    | 2025-03-01T00:00:00Z |
		| Double         | double   |                    5 | Good    | 2025-03-01T00:00:00Z |
		| Float          | float    |                 3.21 | Good    | 2025-03-01T00:00:00Z |
		| Int            | int      |               134567 | Good    | 2025-03-01T00:00:00Z |
		| Long           | long     |              9876543 | Good    | 2025-03-01T00:00:00Z |
		| Short          | short    |                32767 | Good    | 2025-03-01T00:00:00Z |
		| String         | string   | Hi from SuperV!      | Good    | 2025-03-01T00:00:00Z |
		| TimeSpan       | TimeSpan | 01:02:59             | Good    | 2025-03-01T00:00:00Z |
		| Uint           | uint     |               123456 | Good    | 2025-03-01T00:00:00Z |
		| Ulong          | ulong    |             98123456 | Good    | 2025-03-01T00:00:00Z |
		| Ushort         | ushort   |                32767 | Good    | 2025-03-01T00:00:00Z |
		| HistoryTrigger | int      |                    0 | Good    | 2025-03-01T00:00:00Z |

	And Instance "AllFieldsInstance" fields values are updated in project "Project"
		| Name           | Type     | Value                | Quality | Timestamp            |
		| Bool           | bool     | false                | Good    | 2025-03-01T00:59:59Z |
		| DateTime       | DateTime | 2025-03-01T00:59:59Z | Good    | 2025-03-01T00:59:59Z |
		| Double         | double   |                   10 | Good    | 2025-03-01T00:59:59Z |
		| Float          | float    |                 21.3 | Good    | 2025-03-01T00:59:59Z |
		| Int            | int      |              5654321 | Good    | 2025-03-01T00:59:59Z |
		| Long           | long     |              3456789 | Good    | 2025-03-01T00:59:59Z |
		| Short          | short    |               -32767 | Good    | 2025-03-01T00:59:59Z |
		| String         | string   | Bye from SuperV!     | Good    | 2025-03-01T00:59:59Z |
		| TimeSpan       | TimeSpan | 02:03:01             | Good    | 2025-03-01T00:59:59Z |
		| Uint           | uint     |               654321 | Good    | 2025-03-01T00:59:59Z |
		| Ulong          | ulong    |               654789 | Good    | 2025-03-01T00:59:59Z |
		| Ushort         | ushort   |                12768 | Good    | 2025-03-01T00:59:59Z |
		| HistoryTrigger | int      |                    1 | Good    | 2025-03-01T00:59:59Z |

	Then Querying raw history statistics of instance "AllFieldsInstance" in project "Project" between "2025-03-01T00:00:00Z" and "2025-03-01T01:00:00Z" with "01:00:00" interval returns fields statistic values
		| StartTs              | EndTs                | Quality | Double,double,TWA | Double,double,AVG | HistoryTrigger,int,MIN |
		| 2025-03-01T00:00:00Z | 2025-03-01T01:00:00Z | Good    |               7.5 |               7.5 |                      0 |

	And Querying history statistics of instance "AllFieldsInstance" in project "Project" between "2025-03-01T00:00:00Z" and "2025-03-01T01:00:00Z" with "01:00:00" interval returns fields statistic values
		| StartTs              | EndTs                | Quality | Double,double,TWA | Double,double,MIN | Double,double,AVG | Double,double,MAX | Double,long,COUNT | HistoryTrigger,int,MIN |
		| 2025-03-01T00:00:00Z | 2025-03-01T01:00:00Z | Good    |               7.5 |               5.0 |               7.5 |              10.0 |                 2 |                      0 |

	#And Querying raw history statistics of instance "AllFieldsInstance" in project "Project" between "2025-03-01T00:00:00Z" and "2025-03-01T01:00:00Z" with "01:00:00" interval returns fields statistic values
	#	| StartTs              | EndTs                | Quality | Float,float,MAX | Int,int,MAX | Long,long,MAX | Short,short,MAX | String,string,MAX | TimeSpan,TimeSpan,MAX | Uint,uint,MAX | Ulong,ulong,MAX | Ushort,ushort,MAX | HistoryTrigger,int,MAX |
	#	| 2025-03-01T00:00:00Z | 2025-03-01T00:00:00Z | Good    | 3.21            | 134567      | 9876543       | 32767           | Hi from SuperV!   | 01:02:59              | 123456        | 98123456        | 32767             | 1                      |
	#	| 2025-03-01T00:59:59Z | 2025-03-01T00:00:00Z | Good    | 21.3            | 5654321     | 3456789       | -32767          | Bye from SuperV!  | 02:03:01              | 654321        | 654789          | 12768             | 1                      |

	And TDengine is stopped if running
