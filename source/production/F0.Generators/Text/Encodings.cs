using System.Text;

namespace F0.Text;

internal static class Encodings
{
	internal static readonly Encoding Utf8NoBom = new UTF8Encoding(false, true);
}
