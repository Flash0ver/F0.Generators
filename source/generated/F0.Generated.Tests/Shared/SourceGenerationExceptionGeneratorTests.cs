using System.Reflection;
using System.Runtime.Serialization;
using AutoFixture.Xunit2;
using F0.Generated;

namespace F0.Tests.Shared
{
	public class SourceGenerationExceptionGeneratorTests
	{
		[Fact]
		public void IsNotSerializable()
		{
			Type type = typeof(SourceGenerationException);
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
			Type[] parameters = { typeof(SerializationInfo), typeof(StreamingContext) };

			SerializableAttribute? attribute = type.GetCustomAttribute<SerializableAttribute>();
			ConstructorInfo? constructor = type.GetConstructor(bindingAttr, null, parameters, null);

			attribute.Should().BeNull();
			constructor.Should().BeNull();
		}

		[Fact]
		public void Ctor()
		{
			SourceGenerationException exception = new();

			exception.Message.Should().Be(GetNotGeneratedMessage());
			exception.InnerException.Should().BeNull();
			exception.HelpLink.Should().Be(GetHelpLink());
		}

		[Theory, AutoData]
		public void Ctor_String(string message)
		{
			SourceGenerationException exception = new(message);

			exception.Message.Should().BeSameAs(message);
			exception.InnerException.Should().BeNull();
			exception.HelpLink.Should().Be(GetHelpLink());
		}

		[Theory, AutoData]
		public void Ctor_String_Exception(string message, Exception innerException)
		{
			SourceGenerationException exception = new(message, innerException);

			exception.Message.Should().BeSameAs(message);
			exception.InnerException.Should().BeSameAs(innerException);
			exception.HelpLink.Should().Be(GetHelpLink());
		}

		private static string GetNotGeneratedMessage()
		{
			const string message = "The method or operation was not generated correctly."
				+ " Please leave a comment on a related issue, or create a new issue at 'https://github.com/Flash0ver/F0.Generators/issues'."
				+ " Thank you!";

			return message;
		}

		private static string GetHelpLink()
		{
			const string helpLink = "https://github.com/Flash0ver/F0.Generators";

			return helpLink;
		}
	}
}
