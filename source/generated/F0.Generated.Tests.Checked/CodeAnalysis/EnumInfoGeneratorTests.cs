using System.ComponentModel;
using F0.Generated;
using FluentAssertions;
using Xunit;

namespace F0.Tests.Checked.CodeAnalysis
{
	public class EnumInfoGeneratorTests
	{
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

		internal enum ByteEnum : byte { Constant = 1 }
		internal enum SByteEnum : sbyte { Constant = 1 }
		internal enum Int16Enum : short { Constant = 1 }
		internal enum UInt16Enum : ushort { Constant = 1 }
		internal enum Int32Enum : int { Constant = 1 }
		internal enum UInt32Enum : uint { Constant = 1 }
		internal enum Int64Enum : long { Constant = 1 }
		internal enum UInt64Enum : ulong { Constant = 1 }
	}
}
