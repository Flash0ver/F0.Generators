using FluentAssertions;
using Xunit;

namespace F0.Tests.Checked;

public class Assumptions
{
	[Fact]
	public void Assume_That_Checked_Context()
	{
		int max = Int32.MaxValue;
		int min = Int32.MinValue;

		Action overflow = () => max++;
		Action underflow = () => min--;

		overflow.Should().ThrowExactly<OverflowException>();
		underflow.Should().ThrowExactly<OverflowException>();
	}
}
