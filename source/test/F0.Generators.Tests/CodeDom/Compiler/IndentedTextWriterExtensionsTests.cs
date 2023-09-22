using System.CodeDom.Compiler;
using F0.CodeDom.Compiler;

namespace F0.Tests.CodeDom.Compiler;

public class IndentedTextWriterExtensionsTests
{
	[Fact]
	public void WriteLineIndented()
	{
		using StringWriter writer = new();
		using IndentedTextWriter textWriter = new(writer);

		int indent = textWriter.Indent;
		textWriter.WriteLine(0x_F0);

		IndentedTextWriterExtensions.WriteLineIndented(textWriter, "text");

		string text = $"""
			240
			{IndentedTextWriter.DefaultTabString}text

			""";

		Assert.Equal(text, writer.ToString());
		Assert.Equal(indent, textWriter.Indent);
	}

	[Fact]
	public void WriteLineNoTabs()
	{
		using StringWriter writer = new();
		using IndentedTextWriter textWriter = new(writer);

		textWriter.WriteLine();
		textWriter.Indent++;
		textWriter.WriteLine();

		IndentedTextWriterExtensions.WriteLineNoTabs(textWriter);

		string text = $"""

			{IndentedTextWriter.DefaultTabString}


			""";

		Assert.Equal(text, writer.ToString());
		Assert.Equal(1, textWriter.Indent);
	}
}
