using FluentAssertions;

namespace AtelierTomato.Markov.Model.Test
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
			Action act = () => AuthorOID.Parse("Discord:discord.com:142781100152848384:appel");
			act.Should().Throw<ArgumentException>().WithMessage("The OID given has too many members to be a valid AuthorOID. (Parameter 'OID')");
		}
		[Fact]
		public void AuthorParseEmptyTest()
		{
			Action act = () => AuthorOID.Parse(string.Empty);
			act.Should().Throw<ArgumentException>().WithMessage("The OID given is empty (Parameter 'OID')");
		}
		[Fact]
		public void AuthorParseServiceTypeParseFailTest()
		{
			Action act = () => AuthorOID.Parse("We are never using this as a service type lol:google.com:appleseed");
			act.Should().Throw<ArgumentException>().WithMessage("The ServiceType was not able to be parsed from the given OID. (Parameter 'OID')");
		}
		[Fact]
		public void AuthorParseSpecialTest()
		{
			Action act = () => AuthorOID.Parse("Special:google.com:appleseed");
			act.Should().Throw<ArgumentException>().WithMessage("The AuthorOID given is of ServiceType Special, which is not a valid ServiceType for AuthorOIDs. (Parameter 'OID')");
		}
	}
}
