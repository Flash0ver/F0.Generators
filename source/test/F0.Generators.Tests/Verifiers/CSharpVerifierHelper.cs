using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace F0.Tests.Verifiers
{
	internal static class CSharpVerifierHelper
	{
		internal static ImmutableDictionary<string, ReportDiagnostic> NullableWarnings { get; } = GetNullableWarningsFromCompiler();

		private static ImmutableDictionary<string, ReportDiagnostic> GetNullableWarningsFromCompiler()
		{
			string[] args = { "/warnaserror:nullable" };
			CSharpCommandLineArguments commandLineArguments = CSharpCommandLineParser.Default.Parse(args, Environment.CurrentDirectory, Environment.CurrentDirectory);
			ImmutableDictionary<string, ReportDiagnostic> nullableWarnings = commandLineArguments.CompilationOptions.SpecificDiagnosticOptions;

			nullableWarnings = nullableWarnings
				.SetItem("CS8632", ReportDiagnostic.Error)
				.SetItem("CS8669", ReportDiagnostic.Error);

			return nullableWarnings;
		}
	}
}
