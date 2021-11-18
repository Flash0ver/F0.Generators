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
	[SuppressMessage("Usage", "CA2255:The 'ModuleInitializer' attribute should not be used in libraries", Justification = "DEBUG only")]
	internal static class Init
	{
		[ModuleInitializer]
		internal static void RemoveDefaultTraceListener()
			=> Trace.Listeners.Remove("Default");
	}
}
#endif
