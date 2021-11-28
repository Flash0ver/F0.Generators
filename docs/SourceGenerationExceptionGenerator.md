# SourceGenerationException Generator

Generator: [SourceGenerationExceptionGenerator.cs](../source/production/F0.Generators/Shared/SourceGenerationExceptionGenerator.cs)

|                  |                                  |
|------------------|----------------------------------|
| HintName         | `SourceGenerationException.g.cs` |
| Language         | C# 1 or greater                  |
| Target Framework | .NET Standard 1.0 or compatible  |
| Dependencies     | _none_                           |
| Applies to       | `[0.1.0,)`                       |

## Summary

The `F0.Generated.SourceGenerationException` is thrown by other generated code to indicate errors during source generation.

## Remarks

For internal use only.

If you encounter the `F0.Generated.SourceGenerationException` in generated code during runtime, please create a new issue at https://github.com/Flash0ver/F0.Generators/issues, or leave a comment on a related issue.
**Thank you!**

## Diagnostics

### F0GEN0101
Avoid using `SourceGenerationException` directly

Available since _vNext_.

`SourceGenerationException` is intended for internal use by the generators to indicate generation errors or wrong usage.

## Example

```
Unhandled exception.
F0.Generated.SourceGenerationException: The method or operation was not generated correctly.
Please leave a comment on a related issue, or create a new issue at 'https://github.com/Flash0ver/F0.Generators/issues'.
Thank you!
```

## See also

- [Open Issues](https://github.com/Flash0ver/F0.Generators/issues)

## History

- [vNext](../CHANGELOG.md#vNext)
- [0.1.0](../CHANGELOG.md#v010-2021-07-01)
