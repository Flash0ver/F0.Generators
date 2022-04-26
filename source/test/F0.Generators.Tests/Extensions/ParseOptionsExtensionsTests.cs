using F0.Extensions;

namespace F0.Tests.Extensions;

public class ParseOptionsExtensionsTests
{
	[Fact]
	public void IsCSharp_CSharp_True()
	{
		ParseOptions parseOptions = CSharpParseOptions.Default;

		bool isCSharp = parseOptions.IsCSharp();

		Assert.True(isCSharp);
	}

	[Fact]
	public void IsCSharp_VisualBasic_False()
	{
		ParseOptions parseOptions = Microsoft.CodeAnalysis.VisualBasic.VisualBasicParseOptions.Default;

		bool isCSharp = parseOptions.IsCSharp();

		Assert.False(isCSharp);
	}

	[Theory]
	[InlineData(LanguageVersion.CSharp7_3)]
	[InlineData(LanguageVersion.CSharp8)]
	[InlineData(LanguageVersion.CSharp9)]
	public void GetCSharpLanguageVersion_CSharp_DoesNotThrow(LanguageVersion version)
	{
		ParseOptions parseOptions = CSharpParseOptions.Default.WithLanguageVersion(version);

		LanguageVersion langVersion = parseOptions.GetCSharpLanguageVersion();

		Assert.Equal(version, langVersion);
	}

	[Fact]
	public void GetCSharpLanguageVersion_VisualBasic_Throws()
	{
		ParseOptions parseOptions = Microsoft.CodeAnalysis.VisualBasic.VisualBasicParseOptions.Default;

		Func<object> langVersion = () => parseOptions.GetCSharpLanguageVersion();

		Exception exception = Assert.Throws<InvalidCastException>(langVersion);
		Assert.Contains(typeof(CSharpParseOptions).ToString(), exception.Message, StringComparison.Ordinal);
	}
}
