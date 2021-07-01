# F0.Generators
CHANGELOG

## vNext

## v0.1.0 (2021-07-01)
- Added `FriendlyNameGenerator`, which generates both `Friendly.NameOf<T>()` and `Friendly.FullNameOf<T>()`, pretty printing (fully qualified) type names, backed by a reflection-free lookup.
- Added `SourceGenerationExceptionGenerator`, whose `SourceGenerationException` is used by other source generators to indicate errors during source generation.
