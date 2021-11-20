# EnumInfo Generator

Generator: [EnumInfoGenerator.cs](../source/production/F0.Generators/CodeAnalysis/EnumInfoGenerator.cs)

|                  |                                 |
|------------------|---------------------------------|
| HintName         | `EnumInfo.g.cs`                 |
| Language         | C# 2 or greater                 |
| Target Framework | .NET Standard 2.0 or compatible |
| Dependencies     | _none_                          |
| Applies to       | `[0.2.0,)`                      |

## Summary

Allocation-free variants of `System.Enum` methods, with linear search characteristics.

## Remarks

### `F0.Generated.EnumInfo.GetName(System.Enum)`
Generated strongly typed overloads of this placeholder method return a string containing the name of the enumerated constant of its underlying enumeration type; or throw `System.ComponentModel.InvalidEnumArgumentException` if no such constant is found.

## Example

```csharp
using System;
using F0.Generated;

_ = EnumInfo.GetName((Enum)MidpointRounding.AwayFromZero); // -> Cannot use the unspecialized method, which serves as a placeholder for the generator. Enum-Type System.MidpointRounding must be concrete to generate the allocation-free variant of Enum.ToString().
_ = EnumInfo.GetName(PlatformID.Other); //Other
_ = EnumInfo.GetName(ConsoleKey.NumPad0); //NumPad0
_ = EnumInfo.GetName(ConsoleSpecialKey.ControlC); //ControlC
_ = EnumInfo.GetName((TypeCode)18); //String
_ = EnumInfo.GetName((DateTimeKind)byte.MaxValue); // -> The value of argument 'value' (255) is invalid for Enum type 'DateTimeKind'. (Parameter 'value')
_ = EnumInfo.GetName(AttributeTargets.All); //All
_ = EnumInfo.GetName(AttributeTargets.Struct | AttributeTargets.Enum); // -> The value of argument 'value' (24) is invalid for Enum type 'AttributeTargets'. (Parameter 'value')
```

## See also

- [Enumeration types](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/enum)
- [Enum.ToString Method](https://docs.microsoft.com/en-us/dotnet/api/system.enum.tostring)
- [Enum.GetName Method](https://docs.microsoft.com/en-us/dotnet/api/system.enum.getname)
- [Implementation Details Matter, by David Fowler, at iO .NET Virtual Meetup](https://www.youtube.com/watch?v=Cmh5wxM1NkI&t=3150s)
- [Implementation Details Matter, by David Fowler, at Dotnetos Conference 2021](https://www.youtube.com/watch?v=Uyg4_4TZINE&t=2117s)
- [C#'s Enum performance trap your code is suffering from, by Nick Chapsas](https://www.youtube.com/watch?v=BoE5Y6Xkm6w)

## History

- [vNext](../CHANGELOG.md#vNext)
- [0.2.1](../CHANGELOG.md#v021-2021-08-08)
- [0.2.0](../CHANGELOG.md#v020-2021-08-03)
