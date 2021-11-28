# F0.Generators
CHANGELOG

## vNext
- Added a diagnostic to `SourceGenerationExceptionGenerator`, which is reported when the generated `SourceGenerationException` is referenced by user code.
- Added a diagnostic to `EnumInfoGenerator`, which is reported when the unspecialized placeholder method `EnumInfo.GetName(System.Enum)` is invoked by user code.
- Added a diagnostic to `EnumInfoGenerator`, which is reported when the _Global AnalyzerConfig option_ and the _MSBuild property_ to enable generated `EnumInfo.GetName` method overloads to _throw_ an exception instead of returning _null_ are ambiguous.

## v0.4.0 (2021-11-26)
- Added configuration to `EnumInfoGenerator`, to optionally restore the old behavior where `EnumInfo.GetName` throws an exception if the enumerated constant is not found, via either a _global AnalyzerConfig_ or an _MSBuild property_.
- Changed `EnumInfoGenerator`, by default now generating `EnumInfo.GetName` to return _null_ if the enumerated constant is not found, to be on a par with `System.Enum.GetName`.

## v0.3.1 (2021-11-21)
- Fixed exceptions that occurred in both `FriendlyNameGenerator` and `EnumInfoGenerator` when unknown or erroneous arguments were passed.

## v0.3.0 (2021-11-20)
- Changed `EnumInfoGenerator`, additionally generating strongly typed overloads of `EnumInfo.GetName(System.Enum)` for enumeration types with the `System.FlagsAttribute` applied, equivalent to non-Flags.

## v0.2.1 (2021-08-08)
- Fixed `EnumInfoGenerator`, no longer throwing a `System.OverflowException` rather than the intended `System.ComponentModel.InvalidEnumArgumentException`, in case of the default overflow/underflow checking context is set to _checked_ when invoking `EnumInfo.GetName(System.Enum)` with a value which neither has an associated enum member in that enumeration type nor is within the range of `int`.

## v0.2.0 (2021-08-03)
- Added `EnumInfoGenerator`, generating strongly typed overloads of `EnumInfo.GetName(System.Enum)`, which retrieve the name of the constant in the specified non-Flags enumeration type that has the specified value, via an allocation-free linear selection.

## v0.1.1 (2021-07-26)
- Fixed `FriendlyNameGenerator`, now generating `Friendly.NameOf<T>()` to always return a simple name (and the containing types), rather than including namespaces when invoked with a qualified name without a using directive in scope of the call site.

## v0.1.0 (2021-07-01)
- Added `FriendlyNameGenerator`, which generates both `Friendly.NameOf<T>()` and `Friendly.FullNameOf<T>()`, pretty printing (fully qualified) type names, backed by a reflection-free lookup.
- Added `SourceGenerationExceptionGenerator`, whose `SourceGenerationException` is used by other source generators to indicate errors during source generation.
