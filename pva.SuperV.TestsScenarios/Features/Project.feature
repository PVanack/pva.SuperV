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

	And Runnable project is built from WIP project "Project-WIP"
	And Instance "AnInstance" is created with class "TheClass" in project "Project"
		| Name  | Type   | Value |
		| Value | double | 50    |
	And Instance "AnInstance" fields values are updated in project "Project"
		| Name          | Type   | Value |
		| Value         | double | 50    |
		| HighHighLimit | double | 99    |
		| LowLowLimit   | double | 1     |
		| AckState      | string | Unack |

	Then Instance "AnInstance" fields have expected values in project "Project"
		| Name          | Type   | Value | Formatted value |
		| Value         | double | 50    |                 |
		| HighHighLimit | double | 99    |                 |
		| LowLowLimit   | double | 1     |                 |
		| AckState      | int    | 1     | Unack           |
