using AtelierTomato.Markov.Data.Model;
using FluentAssertions;

namespace AtelierTomato.Markov.Data.Test
{
	public class AuthorOIDTest
	{
		[Fact]
		public void AuthorOIDToStringTest()
		{
			AuthorOID author = new(ServiceType.Discord, "discord.com", "142781100152848384");
			author.ToString().Should().Be("Discord:discord.com:142781100152848384");
		}
		[Fact]
		public void AuthorOIDToStringEscapementTest()
		{
			AuthorOID author = new(ServiceType.Discord, "discord:com", "142781100^152848384");
			author.ToString().Should().Be("Discord:discord^:com:142781100^^152848384");
		}

		[Fact]
		public void AuthorParseTest()
		{
			AuthorOID author = AuthorOID.Parse("Discord:discord.com:142781100152848384");
			author.Should().BeEquivalentTo(new AuthorOID(ServiceType.Discord, "discord.com", "142781100152848384"));
		}
		[Fact]
		public void AuthorParseUnescapeTest()
		{
			AuthorOID author = AuthorOID.Parse("Discord:discord^:com:142781100^^152848384");
			author.Should().BeEquivalentTo(new AuthorOID(ServiceType.Discord, "discord:com", "142781100^152848384"));
		}
		[Fact]
		public void AuthorParseTooLongTest()
		{
			var exception = Assert.Throws<ArgumentException>(() => AuthorOID.Parse("Discord:discord.com:142781100152848384:appel"));
			Assert.Equal("The OID given has too many members to be a valid AuthorOID.", exception.Message);
		}
		[Fact]
		public void AuthorParseEmptyTest()
		{
			var exception = Assert.Throws<ArgumentException>(() => AuthorOID.Parse(string.Empty));
			Assert.Equal("The OID given is empty", exception.Message);
		}
		[Fact]
		public void AuthorParseServiceTypeParseFailTest()
		{
			var exception = Assert.Throws<ArgumentException>(() => AuthorOID.Parse("We are never using this as a service type lol:google.com:appleseed"));
			Assert.Equal("The ServiceType was not able to be parsed from the given OID.", exception.Message);
		}
		[Fact]
		public void AuthorParseInvalidTest()
		{
			var exception = Assert.Throws<ArgumentException>(() => AuthorOID.Parse("Invalid:google.com:appleseed"));
			Assert.Equal("The AuthorOID given is of ServiceType Invalid, which is not a valid ServiceType.", exception.Message);
		}
	}
}
