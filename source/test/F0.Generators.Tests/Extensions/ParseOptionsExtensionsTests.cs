using F0.Extensions;

namespace F0.Tests.Extensions
{
	public class ParseOptionsExtensionsTests
	{
		[Fact]
		public void IsCSharp()
		{
			ParseOptions parseOptions = CSharpParseOptions.Default;

			Assert.True(parseOptions.IsCSharp());
		}

		[Fact]
		public void GetCSharpLanguageVersion()
		{
			ParseOptions parseOptions = CSharpParseOptions.Default;

			Assert.Equal(LanguageVersion.CSharp9, parseOptions.GetCSharpLanguageVersion());
		}

		[Theory]
		[InlineData(LanguageVersion.CSharp1, false)]
		[InlineData(LanguageVersion.CSharp2, true)]
		[InlineData(LanguageVersion.CSharp3, true)]
		public void IsCSharp2OrGreater(LanguageVersion languageVersion, bool expected)
		{
			ParseOptions parseOptions = new CSharpParseOptions(languageVersion);

			Assert.Equal(expected, parseOptions.IsCSharp2OrGreater());
		}

		[Theory]
		[InlineData(LanguageVersion.CSharp2, false)]
		[InlineData(LanguageVersion.CSharp3, true)]
		[InlineData(LanguageVersion.CSharp4, true)]
		public void IsCSharp3OrGreater(LanguageVersion languageVersion, bool expected)
		{
			ParseOptions parseOptions = new CSharpParseOptions(languageVersion);

			Assert.Equal(expected, parseOptions.IsCSharp3OrGreater());
		}

		[Theory]
		[InlineData(LanguageVersion.CSharp7_3, false)]
		[InlineData(LanguageVersion.CSharp8, true)]
		[InlineData(LanguageVersion.CSharp9, true)]
		public void IsCSharp8OrGreater(LanguageVersion languageVersion, bool expected)
		{
			ParseOptions parseOptions = new CSharpParseOptions(languageVersion);

			Assert.Equal(expected, parseOptions.IsCSharp8OrGreater());
		}

		[Theory]
		[InlineData(LanguageVersion.CSharp8, false)]
		[InlineData(LanguageVersion.CSharp9, true)]
		[InlineData(LanguageVersion.Latest, true)]
		public void IsCSharp9OrGreater(LanguageVersion languageVersion, bool expected)
		{
			ParseOptions parseOptions = new CSharpParseOptions(languageVersion);

			Assert.Equal(expected, parseOptions.IsCSharp9OrGreater());
		}
	}
}
