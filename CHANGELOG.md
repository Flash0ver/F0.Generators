# F0.Generators
CHANGELOG

## vNext

## v0.1.1 (2021-07-26)
- Fixed `FriendlyNameGenerator`, now generating `Friendly.NameOf<T>()` to always return a simple name (and the containing types), rather than including namespaces when invoked with a qualified name without a using directive in scope of the call site.

## v0.1.0 (2021-07-01)
- Added `FriendlyNameGenerator`, which generates both `Friendly.NameOf<T>()` and `Friendly.FullNameOf<T>()`, pretty printing (fully qualified) type names, backed by a reflection-free lookup.
- Added `SourceGenerationExceptionGenerator`, whose `SourceGenerationException` is used by other source generators to indicate errors during source generation.
