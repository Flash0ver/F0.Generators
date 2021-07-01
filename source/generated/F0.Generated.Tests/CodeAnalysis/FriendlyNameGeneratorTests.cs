using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using F0.Generated;
using FluentAssertions;
using Xunit;

namespace F0.IntegrationTests.CodeAnalysis
{
	public class FriendlyNameGeneratorTests
	{
		[Fact]
		public void Friendly_NameOf_T()
		{
			Friendly.NameOf<object>().Should().Be("object");
			Friendly.NameOf<Action<int[], int[,], int[][]>>().Should().Be("Action<int[], int[,], int[][]>");
			Friendly.NameOf<Tuple<Task, Task<Nullable<int>>>>().Should().Be("Tuple<Task, Task<int?>>");
			Friendly.NameOf<ValueTuple<ValueTask, ValueTask<Nullable<int>>>>().Should().Be("(ValueTask, ValueTask<int?>)");
			Friendly.NameOf<Dictionary<decimal, Func<IEnumerable<decimal?>>>.Enumerator>().Should().Be("Dictionary<decimal, Func<IEnumerable<decimal?>>>.Enumerator");
		}

		[Fact]
		public void Friendly_FullNameOf_T()
		{
			Friendly.FullNameOf<object>().Should().Be("object");
			Friendly.FullNameOf<Action<int[], int[,], int[][]>>().Should().Be("System.Action<int[], int[,], int[][]>");
			Friendly.FullNameOf<Tuple<Task, Task<Nullable<int>>>>().Should().Be("System.Tuple<System.Threading.Tasks.Task, System.Threading.Tasks.Task<int?>>");
			Friendly.FullNameOf<ValueTuple<ValueTask, ValueTask<Nullable<int>>>>().Should().Be("(System.Threading.Tasks.ValueTask, System.Threading.Tasks.ValueTask<int?>)");
			Friendly.FullNameOf<Dictionary<decimal, Func<IEnumerable<decimal?>>>.Enumerator>().Should().Be("System.Collections.Generic.Dictionary<decimal, System.Func<System.Collections.Generic.IEnumerable<decimal?>>>.Enumerator");
		}
	}
}
