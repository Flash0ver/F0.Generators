# Friendly Name Generator

Generator: [FriendlyNameGenerator.cs](../source/production/F0.Generators/CodeAnalysis/FriendlyNameGenerator.cs)

|                  |                                 |
|------------------|---------------------------------|
| HintName         | `Friendly.g.cs`                 |
| Language         | C# 7.3 or greater               |
| Target Framework | .NET Standard 1.0 or compatible |
| Dependencies     | _none_                          |
| Applies to       | `[0.1.0,)`                      |

## Summary

Pretty printing of (fully qualified) type names, backed by a reflection-free lookup.

## Remarks

### `F0.Generated.Friendly.NameOf<T>()`
Pretty prints the _simple name_ of a type.

### `F0.Generated.Friendly.FullNameOf<T>()`
Pretty prints the _fully qualified name_ of a type, including its namespace but not its assembly.

There is no variant pretty printing the _assembly-qualified name_ of a type.

## Example

```csharp
using System;
using System.Collections.Generic;
using System.Numerics;
using F0.Generated;

_ = Friendly.NameOf<Dictionary<int, bool?>.Enumerator>(); //Dictionary<int, bool?>.Enumerator
_ = Friendly.NameOf<Tuple<string[], string[,], string[][]>>(); //Tuple<string[], string[,], string[][]>
_ = Friendly.NameOf<(BigInteger First, Complex Second)>(); //(BigInteger, Complex)

_ = Friendly.FullNameOf<Dictionary<int, bool?>.Enumerator>(); //System.Collections.Generic.Dictionary<int, bool?>.Enumerator
_ = Friendly.FullNameOf<Tuple<string[], string[,], string[][]>>(); //System.Tuple<string[], string[,], string[][]>
_ = Friendly.FullNameOf<(BigInteger First, Complex Second)>(); //(System.Numerics.BigInteger, System.Numerics.Complex)
```

## See also

- [MemberInfo.Name Property](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.memberinfo.name)
- [Type.FullName Property](https://docs.microsoft.com/en-us/dotnet/api/system.type.fullname)
- [Type.AssemblyQualifiedName Property](https://docs.microsoft.com/en-us/dotnet/api/system.type.assemblyqualifiedname)

## History

- [vNext](../CHANGELOG.md#vNext)
- [0.3.1](../CHANGELOG.md#v031-2021-11-21)
- [0.1.1](../CHANGELOG.md#v011-2021-07-26)
- [0.1.0](../CHANGELOG.md#v010-2021-07-01)
