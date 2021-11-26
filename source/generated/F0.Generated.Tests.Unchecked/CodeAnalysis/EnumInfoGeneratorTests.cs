using System.ComponentModel;
using F0.Generated;

namespace F0.Tests.Unchecked.CodeAnalysis;

public class EnumInfoGeneratorTests
{
	[Fact]
	public void GetName_Enum_Throws()
	{
		Enum @enum = Enumeration.Single;
		string message = "Cannot use the unspecialized method, which serves as a placeholder for the generator." +
			$" Enum-Type F0.Tests.Unchecked.CodeAnalysis.EnumInfoGeneratorTests+Enumeration must be concrete to generate the allocation-free variant of Enum.ToString().";

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

	[Fact]
	public void GetName_Enumeration_IsDefined_TheNameOfTheEnumeratedConstant()
	{
		Enumeration @enum = Enumeration.Single;

#if HAS_GENERIC_ENUM_GETNAME
		Enum.IsDefined<Enumeration>(@enum).Should().BeTrue();
#else
		Enum.IsDefined(typeof(Enumeration), @enum).Should().BeTrue();
#endif

		string actual = EnumInfo.GetName(@enum);

		actual.Should().Be(nameof(Enumeration.Single), "generated");
		actual.Should().Be(@enum.ToString(), nameof(@enum.ToString));

#if HAS_GENERIC_ENUM_GETNAME
		actual.Should().Be(Enum.GetName<Enumeration>(@enum), nameof(Enum.GetName));
#else
		actual.Should().Be(Enum.GetName(typeof(Enumeration), @enum), nameof(Enum.GetName));
#endif
	}

	[Fact]
	public void GetName_Enumeration_IsNotDefined_NoEnumeratedConstantIsFound()
	{
		var @enum = (Enumeration)0xF0;

#if HAS_GENERIC_ENUM_GETNAME
		Enum.IsDefined<Enumeration>(@enum).Should().BeFalse();
#else
		Enum.IsDefined(typeof(Enumeration), @enum).Should().BeFalse();
#endif

		string message = $"The value of argument 'value' (240) is invalid for Enum type '{nameof(Enumeration)}'.";

#if NET
		message += " (Parameter 'value')";
#else
		message += Environment.NewLine + "Parameter name: value";
#endif

		Func<string> getName = () => EnumInfo.GetName(@enum);

		getName.Should().ThrowExactly<InvalidEnumArgumentException>()
			.WithMessage(message).And
			.ParamName.Should().Be("value");

		@enum.ToString().Should().Be("240", nameof(@enum.ToString));

#if HAS_GENERIC_ENUM_GETNAME
		Enum.GetName<Enumeration>(@enum).Should().BeNull(nameof(Enum.GetName));
#else
		Enum.GetName(typeof(Enumeration), @enum).Should().BeNull(nameof(Enum.GetName));
#endif
	}

	[Theory]
	[InlineData(Flags.None, "None")]
	[InlineData(Flags.First, "First")]
	[InlineData(Flags.Second, "Second")]
	[InlineData(Flags.Third, "Third")]
	[InlineData(Flags.Fourth, "Fourth")]
	[InlineData(Flags.All, "All")]
	public void GetName_Flags_IsAvailable_TheNameOfTheEnumeratedConstant(Flags flags, string expected)
	{
#if HAS_GENERIC_ENUM_GETNAME
		Enum.IsDefined<Flags>(flags).Should().BeTrue();
#else
		Enum.IsDefined(typeof(Flags), flags).Should().BeTrue();
#endif

		string actual = EnumInfo.GetName(flags);

		actual.Should().Be(expected, "generated");
		actual.Should().Be(flags.ToString(), nameof(flags.ToString));

#if HAS_GENERIC_ENUM_GETNAME
		actual.Should().Be(Enum.GetName<Flags>(flags), nameof(Enum.GetName));
#else
		actual.Should().Be(Enum.GetName(typeof(Flags), flags), nameof(Enum.GetName));
#endif
	}

	[Theory]
	[InlineData(3, Flags.First | Flags.Second, "First, Second")]
	[InlineData(5, Flags.First | Flags.Third, "First, Third")]
	[InlineData(9, Flags.First | Flags.Fourth, "First, Fourth")]
	public void GetName_Flags_IsUnavailable_NoEnumeratedConstantIsFound(int value, Flags flags, string expected)
	{
		((int)flags).Should().Be(value);

#if HAS_GENERIC_ENUM_GETNAME
		Enum.IsDefined<Flags>(flags).Should().BeFalse();
#else
		Enum.IsDefined(typeof(Flags), flags).Should().BeFalse();
#endif

		string message = $"The value of argument 'value' ({value}) is invalid for Enum type '{nameof(Flags)}'.";

#if NET
		message += " (Parameter 'value')";
#else
		message += Environment.NewLine + "Parameter name: value";
#endif

		Func<string> getName = () => EnumInfo.GetName(flags);

		getName.Should().ThrowExactly<InvalidEnumArgumentException>()
			.WithMessage(message).And
			.ParamName.Should().Be("value");

		flags.ToString().Should().Be(expected, nameof(flags.ToString));

#if HAS_GENERIC_ENUM_GETNAME
		Enum.GetName<Flags>(flags).Should().BeNull(nameof(Enum.GetName));
#else
		Enum.GetName(typeof(Flags), flags).Should().BeNull(nameof(Enum.GetName));
#endif
	}

	[Fact]
	public void No_System_OverflowException()
	{
		var minByte = (ByteEnum)Byte.MinValue;
		var maxByte = (ByteEnum)Byte.MaxValue;

		var minSByte = (SByteEnum)SByte.MinValue;
		var maxSByte = (SByteEnum)SByte.MaxValue;

		var minInt16 = (Int16Enum)Int16.MinValue;
		var maxInt16 = (Int16Enum)Int16.MaxValue;

		var minUInt16 = (UInt16Enum)UInt16.MinValue;
		var maxUInt16 = (UInt16Enum)UInt16.MaxValue;

		var minInt32 = (Int32Enum)Int32.MinValue;
		var maxInt32 = (Int32Enum)Int32.MaxValue;

		var minUInt32 = (UInt32Enum)UInt32.MinValue;
		var maxUInt32 = (UInt32Enum)UInt32.MaxValue;

		var minInt64 = (Int64Enum)Int64.MinValue;
		var maxInt64 = (Int64Enum)Int64.MaxValue;

		var minUInt64 = (UInt64Enum)UInt64.MinValue;
		var maxUInt64 = (UInt64Enum)UInt64.MaxValue;

		Assert(minByte, () => EnumInfo.GetName(minByte), typeof(byte), Byte.MinValue);
		Assert(maxByte, () => EnumInfo.GetName(maxByte), typeof(byte), Byte.MaxValue);

		Assert(minSByte, () => EnumInfo.GetName(minSByte), typeof(sbyte), SByte.MinValue);
		Assert(maxSByte, () => EnumInfo.GetName(maxSByte), typeof(sbyte), SByte.MaxValue);

		Assert(minInt16, () => EnumInfo.GetName(minInt16), typeof(short), Int16.MinValue);
		Assert(maxInt16, () => EnumInfo.GetName(maxInt16), typeof(short), Int16.MaxValue);

		Assert(minUInt16, () => EnumInfo.GetName(minUInt16), typeof(ushort), UInt16.MinValue);
		Assert(maxUInt16, () => EnumInfo.GetName(maxUInt16), typeof(ushort), UInt16.MaxValue);

		Assert(minInt32, () => EnumInfo.GetName(minInt32), typeof(int), Int32.MinValue);
		Assert(maxInt32, () => EnumInfo.GetName(maxInt32), typeof(int), Int32.MaxValue);

		Assert(minUInt32, () => EnumInfo.GetName(minUInt32), typeof(uint), 0);
		Assert(maxUInt32, () => EnumInfo.GetName(maxUInt32), typeof(uint), -1);

		Assert(minInt64, () => EnumInfo.GetName(minInt64), typeof(long), 0);
		Assert(maxInt64, () => EnumInfo.GetName(maxInt64), typeof(long), -1);

		Assert(minUInt64, () => EnumInfo.GetName(minUInt64), typeof(ulong), 0);
		Assert(maxUInt64, () => EnumInfo.GetName(maxUInt64), typeof(ulong), -1);

		static void Assert<TEnum>(TEnum value, Func<string> getName, Type underlyingType, int invalidValue)
			where TEnum : struct, Enum
		{
			typeof(TEnum).GetEnumUnderlyingType().Should().Be(underlyingType);

			Check_That_Enum_IsNotDefined<TEnum>(value);
			Check_That_Exception_IsNotThrown(getName);
			Check_That_Exception_IsThrown<TEnum>(getName, invalidValue);
		}
	}

	private static void Check_That_Enum_IsNotDefined<TEnum>(TEnum value)
		where TEnum : struct, Enum
#if HAS_GENERIC_ENUM_GETNAME
		=> Enum.IsDefined<TEnum>(value).Should().BeFalse();
#else
		=> Enum.IsDefined(typeof(TEnum), value).Should().BeFalse();
#endif

	private static void Check_That_Exception_IsNotThrown(Func<string> getName)
		=> getName.Should().NotThrow<OverflowException>();

	private static void Check_That_Exception_IsThrown<TEnum>(Func<string> getName, int invalidValue)
		where TEnum : struct, Enum
	{
		string message = $"The value of argument 'value' ({invalidValue}) is invalid for Enum type '{typeof(TEnum).Name}'.";

#if NET
		message += " (Parameter 'value')";
#else
		message += Environment.NewLine + "Parameter name: value";
#endif

		getName.Should().ThrowExactly<InvalidEnumArgumentException>($"'{typeof(TEnum).Name}'")
			.WithMessage(message).And
			.ParamName.Should().Be("value");
	}

	internal enum Enumeration
	{
		Single,
	}

	[Flags]
	public enum Flags
	{
		None = 0b_0000,
		First = 0b_0001,
		Second = 0b_0010,
		Third = 0b_0100,
		Fourth = 0b_1000,
		All = First | Second | Third | Fourth,
	}

	internal enum ByteEnum : byte { Constant = 1 }
	internal enum SByteEnum : sbyte { Constant = 1 }
	internal enum Int16Enum : short { Constant = 1 }
	internal enum UInt16Enum : ushort { Constant = 1 }
	internal enum Int32Enum : int { Constant = 1 }
	internal enum UInt32Enum : uint { Constant = 1 }
	internal enum Int64Enum : long { Constant = 1 }
	internal enum UInt64Enum : ulong { Constant = 1 }
}
