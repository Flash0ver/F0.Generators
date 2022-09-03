using System.Diagnostics.CodeAnalysis;

namespace F0.Tests.CSharp;

[SuppressMessage("Performance", "CA1802:Use literals where appropriate", Justification = "Testing")]
internal static class LanguageFeatures
{
	//C# 2
	public static readonly string NamespaceAliasQualifier = "namespace alias qualifier";

	//C# 3
	public static readonly string CollectionInitializer = "collection initializer";

	//C# 6
	public static readonly string InterpolatedStrings = "interpolated strings";
	public static readonly string NameofOperator = "nameof operator";
	public static readonly string NullPropagatingOperator = "null propagating operator";
}
