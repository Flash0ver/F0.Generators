using System;
using System.ComponentModel;
using System.Reflection;
using F0.Generated;
using FluentAssertions;
using Xunit;

namespace F0.Tests.CodeAnalysis
{
	public class EnumInfoGeneratorTests
	{
		[Fact]
		public void GetName_Enum_Throws()
		{
			Enum @enum = DayOfWeek.Sunday;
			string message = "Cannot use the unspecialized method, which serves as a placeholder for the generator." +
				$" Enum-Type System.DayOfWeek must be concrete to generate the allocation-free variant of Enum.ToString().";

			Func<string> getName = () => EnumInfo.GetName(@enum);

			getName.Should().ThrowExactly<SourceGenerationException>()
				.WithMessage(message);
		}

		[Fact]
		public void GetName_Null_Throws()
		{
			string message = "Cannot use the unspecialized method, which serves as a placeholder for the generator." +
				$" Enum-Type <null> must be concrete to generate the allocation-free variant of Enum.ToString().";

			Func<string> getName = () => EnumInfo.GetName(null);

			getName.Should().ThrowExactly<SourceGenerationException>()
				.WithMessage(message);
		}

		[Theory]
		[InlineData(DayOfWeek.Sunday, nameof(DayOfWeek.Sunday))]
		[InlineData(DayOfWeek.Monday, nameof(DayOfWeek.Monday))]
		[InlineData(DayOfWeek.Tuesday, nameof(DayOfWeek.Tuesday))]
		[InlineData(DayOfWeek.Wednesday, nameof(DayOfWeek.Wednesday))]
		[InlineData(DayOfWeek.Thursday, nameof(DayOfWeek.Thursday))]
		[InlineData(DayOfWeek.Friday, nameof(DayOfWeek.Friday))]
		[InlineData(DayOfWeek.Saturday, nameof(DayOfWeek.Saturday))]
		public void GetName_IsDefined_TheNameOfTheEnumeratedConstant(DayOfWeek @enum, string expected)
		{
#if HAS_GENERIC_ENUM_GETNAME
			Enum.IsDefined<DayOfWeek>(@enum).Should().BeTrue();
#else
			Enum.IsDefined(typeof(DayOfWeek), @enum).Should().BeTrue();
#endif

			string actual = EnumInfo.GetName(@enum);

			actual.Should().Be(expected, "generated");
			actual.Should().Be(@enum.ToString(), nameof(@enum.ToString));

#if HAS_GENERIC_ENUM_GETNAME
			actual.Should().Be(Enum.GetName<DayOfWeek>(@enum), nameof(Enum.GetName));
#else
			actual.Should().Be(Enum.GetName(typeof(DayOfWeek), @enum), nameof(Enum.GetName));
#endif
		}

		[Fact]
		public void GetName_IsNotDefined_NoEnumeratedConstantIsFound()
		{
			var @enum = (DayOfWeek)0xF0;

#if HAS_GENERIC_ENUM_GETNAME
			Enum.IsDefined<DayOfWeek>(@enum).Should().BeFalse();
#else
			Enum.IsDefined(typeof(DayOfWeek), @enum).Should().BeFalse();
#endif

			string message = $"The value of argument 'value' (240) is invalid for Enum type '{nameof(DayOfWeek)}'.";

#if NET
			message += " (Parameter 'value')";
#else
			message += Environment.NewLine + "Parameter name: value";
#endif

			Func<string> getName = () => EnumInfo.GetName(@enum);

			getName.Should().ThrowExactly<InvalidEnumArgumentException>()
				.WithMessage(message).And
				.ParamName.Should().Be("value");
		}

		[Fact]
		public void GetName_Flags_Throws()
		{
			MemberTypes @enum = MemberTypes.Method;
			string message = "Flags are not yet supported: see https://github.com/Flash0ver/F0.Generators/issues/1";

			Func<string> getName = () => EnumInfo.GetName(@enum);

			getName.Should().ThrowExactly<SourceGenerationException>()
				.WithMessage(message);
		}
	}
}
