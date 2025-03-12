# Field value formatting
[Field value formatting](FieldFormatter.cs) allows to format the value of the associated field to a string in a particular format. The already defined formatters are the following:
- [EnumFormatter](#Enum-formatter)
A field value formatter can restrict the type of field on which it can be applied through *AllowedTypes*.
 
## Enum formatter
The [enum formatter](EnumFormatter.cs) allows to define a list of strings associated with integer values (for example to format the value to OFF when the value is 0 and to ON when the value is 1).
It's only allowed on  the following field types:
- short
- ushort
- int
- uint
- long
- ulong

Its associated properties are:
- *enumName* The name of the formatter. It's easier to name them after the string values contained in enum. For example *OFF/ON* or *Plant-Areas*.
- *values* The string values associated with enum. They can be specified either as a *HashSet* if the first string is associated to 0 and the values increase by 1 or a *dictionary* where you specify the integer value and associated string.