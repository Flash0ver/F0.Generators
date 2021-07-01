using System.CodeDom.Compiler;

namespace F0.CodeDom.Compiler
{
	internal static class IndentedTextWriterExtensions
	{
		internal static void WriteLineIndented(this IndentedTextWriter textWriter, string text)
		{
			textWriter.Indent++;
			textWriter.WriteLine(text);
			textWriter.Indent--;
		}

		internal static void WriteLineNoTabs(this IndentedTextWriter textWriter)
		{
			textWriter.WriteLineNoTabs(null);
		}
	}
}
