#if DEBUG && !NETCOREAPP
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	internal sealed class ModuleInitializerAttribute : Attribute
	{
		internal ModuleInitializerAttribute()
		{
		}
	}
}

namespace F0
{
	[SuppressMessage("Usage", "CA2255:The 'ModuleInitializer' attribute should not be used in libraries", Justification = "DEBUG && NETFRAMEWORK only")]
	internal static class Init
	{
		[ModuleInitializer]
		internal static void RemoveDefaultTraceListener()
		{
			Trace.Listeners.Remove("Default");
			int position = Trace.Listeners.Add(new DebugTraceListener());
			if (position != 0)
			{
				throw new InvalidOperationException($"Replacement for {nameof(DefaultTraceListener)} is not exclusive.");
			}
		}
	}

	internal sealed class DebugTraceListener : TraceListener
	{
		public DebugTraceListener()
			: base(nameof(Debug))
		{ }

		public DebugTraceListener(string? name)
			: base(name)
		{ }

		public override void Write(string? message)
			=> throw new NotImplementedException($"{nameof(DebugTraceListener)}.{nameof(Write)}({nameof(String)})");

		public override void WriteLine(string? message)
			=> throw new DebugAssertException(message);
	}

	[SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "DEBUG && NETFRAMEWORK only")]
	[SuppressMessage("Design", "CA1064:Exceptions should be public", Justification = "DEBUG && NETFRAMEWORK only")]
	internal sealed class DebugAssertException : Exception
	{
		public DebugAssertException(string? message)
			: base(message)
		{ }
	}
}
#endif
