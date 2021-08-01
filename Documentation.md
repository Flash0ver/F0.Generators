# Documentation
F0.Generators

All types of `F0.Generators` are _generated_ for the _C#_ language, members declared in the _namespace_ `F0.Generated`, and have `internal` _accessibility_:

### [FriendlyNameGenerator](./docs/FriendlyNameGenerator.md)
Pretty printing of (fully qualified) type names, backed by a reflection-free lookup.

### [EnumInfoGenerator](./docs/EnumInfoGenerator.md)
Allocation-free variants of `System.Enum` methods, with linear search characteristics.

### [SourceGenerationExceptionGenerator](./docs/SourceGenerationExceptionGenerator.md)
`SourceGenerationException` is thrown by other generated code to indicate errors during source generation.
For internal use only.
