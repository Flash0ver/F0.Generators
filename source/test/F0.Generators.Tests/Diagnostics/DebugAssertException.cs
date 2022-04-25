#if DEBUG && NETFRAMEWORK
using System.Diagnostics.CodeAnalysis;

namespace F0.Tests.Diagnostics;

[SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "DEBUG && NETFRAMEWORK")]
[SuppressMessage("Design", "CA1064:Exceptions should be public", Justification = "DEBUG && NETFRAMEWORK")]
internal sealed class DebugAssertException : Exception
{
	public DebugAssertException(string? message)
		: base(message)
	{
	}
}
#endif
