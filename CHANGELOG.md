# F0.Generators
CHANGELOG

## vNext
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
