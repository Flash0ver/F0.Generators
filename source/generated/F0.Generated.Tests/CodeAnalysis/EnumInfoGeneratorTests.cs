using System.Reflection;
using F0.Generated;

namespace F0.Tests.CodeAnalysis;

public class EnumInfoGeneratorTests
{
	[Fact]
	public void GetName_Enum_Throws()
	{
		Enum @enum = DayOfWeek.Sunday;
		string message = "Cannot use the unspecialized method, which serves as a placeholder for the generator." +
			" Enum-Type System.DayOfWeek must be concrete to generate the allocation-free variant of Enum.ToString().";

		Func<string?> getName = () => EnumInfo.GetName(@enum);

		getName.Should().ThrowExactly<SourceGenerationException>()
			.WithMessage(message);
	}

	[Fact]
	public void GetName_Null_Throws()
	{
		string message = "Cannot use the unspecialized method, which serves as a placeholder for the generator." +
			" Enum-Type <null> must be concrete to generate the allocation-free variant of Enum.ToString().";

		Func<string?> getName = () => EnumInfo.GetName(null);

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
	public void GetName_Enumeration_IsDefined_TheNameOfTheEnumeratedConstant(DayOfWeek @enum, string expected)
	{
#if HAS_GENERIC_ENUM_GETNAME
		Enum.IsDefined<DayOfWeek>(@enum).Should().BeTrue();
#else
		Enum.IsDefined(typeof(DayOfWeek), @enum).Should().BeTrue();
#endif

		string? actual = EnumInfo.GetName(@enum);

		actual.Should().Be(expected, "generated");
		actual.Should().Be(@enum.ToString(), nameof(@enum.ToString));

#if HAS_GENERIC_ENUM_GETNAME
		actual.Should().Be(Enum.GetName<DayOfWeek>(@enum), nameof(Enum.GetName));
#else
		actual.Should().Be(Enum.GetName(typeof(DayOfWeek), @enum), nameof(Enum.GetName));
#endif
	}

	[Fact]
	public void GetName_Enumeration_IsNotDefined_NoEnumeratedConstantIsFound()
	{
		var @enum = (DayOfWeek)0xF0;

#if HAS_GENERIC_ENUM_GETNAME
		Enum.IsDefined<DayOfWeek>(@enum).Should().BeFalse();
#else
		Enum.IsDefined(typeof(DayOfWeek), @enum).Should().BeFalse();
#endif

		string? actual = EnumInfo.GetName(@enum);

		actual.Should().BeNull("generated");
		@enum.ToString().Should().Be("240", nameof(@enum.ToString));

#if HAS_GENERIC_ENUM_GETNAME
		Enum.GetName<DayOfWeek>(@enum).Should().BeNull(nameof(Enum.GetName));
#else
		Enum.GetName(typeof(DayOfWeek), @enum).Should().BeNull(nameof(Enum.GetName));
#endif
	}

	[Theory]
	[InlineData(ResourceLocation.Embedded, "Embedded")]
	[InlineData(ResourceLocation.ContainedInAnotherAssembly, "ContainedInAnotherAssembly")]
	[InlineData(ResourceLocation.ContainedInManifestFile, "ContainedInManifestFile")]
	public void GetName_Flags_IsAvailable_TheNameOfTheEnumeratedConstant(ResourceLocation flags, string expected)
	{
#if HAS_GENERIC_ENUM_GETNAME
		Enum.IsDefined<ResourceLocation>(flags).Should().BeTrue();
#else
		Enum.IsDefined(typeof(ResourceLocation), flags).Should().BeTrue();
#endif

		string? actual = EnumInfo.GetName(flags);

		actual.Should().Be(expected, "generated");
		actual.Should().Be(flags.ToString(), nameof(flags.ToString));

#if HAS_GENERIC_ENUM_GETNAME
		actual.Should().Be(Enum.GetName<ResourceLocation>(flags), nameof(Enum.GetName));
#else
		actual.Should().Be(Enum.GetName(typeof(ResourceLocation), flags), nameof(Enum.GetName));
#endif
	}

	[Theory]
	[InlineData(3, ResourceLocation.Embedded | ResourceLocation.ContainedInAnotherAssembly, "Embedded, ContainedInAnotherAssembly")]
	[InlineData(5, ResourceLocation.Embedded | ResourceLocation.ContainedInManifestFile, "Embedded, ContainedInManifestFile")]
	[InlineData(6, ResourceLocation.ContainedInAnotherAssembly | ResourceLocation.ContainedInManifestFile, "ContainedInAnotherAssembly, ContainedInManifestFile")]
	[InlineData(7, ResourceLocation.Embedded | ResourceLocation.ContainedInAnotherAssembly | ResourceLocation.ContainedInManifestFile, "Embedded, ContainedInAnotherAssembly, ContainedInManifestFile")]
	public void GetName_Flags_IsUnavailable_NoEnumeratedConstantIsFound(int value, ResourceLocation flags, string expected)
	{
		((int)flags).Should().Be(value);

#if HAS_GENERIC_ENUM_GETNAME
		Enum.IsDefined<ResourceLocation>(flags).Should().BeFalse();
#else
		Enum.IsDefined(typeof(ResourceLocation), flags).Should().BeFalse();
#endif

		string? actual = EnumInfo.GetName(flags);

		actual.Should().BeNull("generated");
		flags.ToString().Should().Be(expected, nameof(flags.ToString));

#if HAS_GENERIC_ENUM_GETNAME
		Enum.GetName<ResourceLocation>(flags).Should().BeNull(nameof(Enum.GetName));
#else
		Enum.GetName(typeof(ResourceLocation), flags).Should().BeNull(nameof(Enum.GetName));
#endif
	}
}
