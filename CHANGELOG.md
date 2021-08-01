# F0.Generators
CHANGELOG

## vNext
- Added `EnumInfoGenerator`, generating strongly typed overloads of `EnumInfo.GetName(System.Enum)`, which retrieve the name of the constant in the specified non-Flags enumeration type that has the specified value, via an allocation-free linear selection.

## v0.1.1 (2021-07-26)
- Fixed `FriendlyNameGenerator`, now generating `Friendly.NameOf<T>()` to always return a simple name (and the containing types), rather than including namespaces when invoked with a qualified name without a using directive in scope of the call site.

## v0.1.0 (2021-07-01)
- Added `FriendlyNameGenerator`, which generates both `Friendly.NameOf<T>()` and `Friendly.FullNameOf<T>()`, pretty printing (fully qualified) type names, backed by a reflection-free lookup.
- Added `SourceGenerationExceptionGenerator`, whose `SourceGenerationException` is used by other source generators to indicate errors during source generation.
