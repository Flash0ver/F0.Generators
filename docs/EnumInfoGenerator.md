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
Generated strongly typed overloads of this placeholder method return a string containing the name of the enumerated constant of its underlying enumeration type; or _null_ if no such constant is found.

#### Configuration
To throw `System.ComponentModel.InvalidEnumArgumentException` instead of returning _null_ if no such constant is found, set the _MSBuild property_ `F0Gen_EnumInfo_ThrowIfConstantNotFound` to `enable` &#8209;or&#8209; set `f0gen_enum_throw` to `true` in a _global AnalyzerConfig_:

##### MSBuild (e.g. `.csproj`, `.props`, `.targets`)
```xml
<Project>
  <PropertyGroup>
    <F0Gen_EnumInfo_ThrowIfConstantNotFound>enable</F0Gen_EnumInfo_ThrowIfConstantNotFound>
  </PropertyGroup>
</Project>
```

##### Global AnalyzerConfig (e.g. `.globalconfig`)
```ini
is_global = true
f0gen_enum_throw = true
```

## Diagnostics

### F0GEN0301
Do not use the unspecialized placeholder method of `EnumInfo.GetName(Enum)`

Available since _vNext_.

`EnumInfo.GetName(Enum)` does not return.

### F0GEN0302
Ambiguous configuration of `EnumInfoGenerator`

Available since _vNext_.

The _globalconfig_ option `f0gen_enum_throw` and the _MSBuild_ property `F0Gen_EnumInfo_ThrowIfConstantNotFound` are ambiguous.

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
- [0.4.0](../CHANGELOG.md#v040-2021-11-26)
- [0.3.1](../CHANGELOG.md#v031-2021-11-21)
- [0.3.0](../CHANGELOG.md#v030-2021-11-20)
- [0.2.1](../CHANGELOG.md#v021-2021-08-08)
- [0.2.0](../CHANGELOG.md#v020-2021-08-03)
