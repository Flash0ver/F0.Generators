# Documentation
F0.Generators

- [Generators](#generators)
- [Diagnostics](#diagnostics)

## Generators

All types of `F0.Generators` are _generated_ for the _C#_ language, members declared in the _namespace_ `F0.Generated`, and have `internal` _accessibility_:

### [SourceGenerationExceptionGenerator](./docs/SourceGenerationExceptionGenerator.md)
`SourceGenerationException` is thrown by other generated code to indicate errors during source generation.
For internal use only.

### [FriendlyNameGenerator](./docs/FriendlyNameGenerator.md)
Pretty printing of (fully qualified) type names, backed by a reflection-free lookup.

### [EnumInfoGenerator](./docs/EnumInfoGenerator.md)
Allocation-free variants of `System.Enum` methods, with linear search characteristics.

## Diagnostics

| Diagnostic ID                                                       | Category                                                                           | Severity | Description                                                                 |
| ------------------------------------------------------------------- | ---------------------------------------------------------------------------------- | -------- | --------------------------------------------------------------------------- |
| [F0GEN0101](./docs/SourceGenerationExceptionGenerator.md#F0GEN0101) | [SourceGenerationExceptionGenerator](./docs/SourceGenerationExceptionGenerator.md) | Warning  | Avoid using `SourceGenerationException` directly                            |
| [F0GEN0301](./docs/EnumInfoGenerator.md#F0GEN0301)                  | [EnumInfoGenerator](./docs/EnumInfoGenerator.md)                                   | Error    | Do not use the unspecialized placeholder method of `EnumInfo.GetName(Enum)` |
| [F0GEN0302](./docs/EnumInfoGenerator.md#F0GEN0302)                  | [EnumInfoGenerator](./docs/EnumInfoGenerator.md)                                   | Warning  | Ambiguous configuration of `EnumInfoGenerator`                              |

[History](./source/production/F0.Generators/ReleaseTracking/AnalyzerReleases.Shipped.md)
