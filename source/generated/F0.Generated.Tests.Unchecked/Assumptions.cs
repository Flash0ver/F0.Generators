using FluentAssertions;
using Xunit;

namespace F0.Tests.Unchecked
{
	public class Assumptions
	{
		[Fact]
		public void Assume_That_Unchecked_Context()
		{
			int max = Int32.MaxValue;
			int min = Int32.MinValue;

			Action overflow = () => max++;
			Action underflow = () => min--;

			overflow.Should().NotThrow<OverflowException>();
			underflow.Should().NotThrow<OverflowException>();
		}
	}
}
