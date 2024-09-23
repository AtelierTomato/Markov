using AtelierTomato.Markov.Model.ObjectOID;
using AtelierTomato.Markov.Model.ObjectOID.Parser;
using FluentAssertions;

namespace AtelierTomato.Markov.Model.Test
{
	public class MultiParserTest
	{
		[Fact]
		public void DiscordIObjectOIDParseTest()
		{
			MultiParser<IObjectOID> oidParser = new([new BookObjectOIDParser(), new SpecialObjectOIDParser(), new DiscordObjectOIDParser()]);
			IObjectOID OID = oidParser.Parse("Discord:discord.com:1253189664655806606:1253189664655806610");
			OID.Should().BeOfType<DiscordObjectOID>();
			OID.Should().BeEquivalentTo(DiscordObjectOID.Parse("Discord:discord.com:1253189664655806606:1253189664655806610"));
		}
		[Fact]
		public void BookObjectOIDParseTest()
		{
			MultiParser<IObjectOID> oidParser = new([new BookObjectOIDParser(), new SpecialObjectOIDParser(), new DiscordObjectOIDParser()]);
			IObjectOID OID = oidParser.Parse("Book:_:Alice in Wonderland:Through the Looking Glass:1");
			OID.Should().BeOfType<BookObjectOID>();
			OID.Should().BeEquivalentTo(BookObjectOID.Parse("Book:_:Alice in Wonderland:Through the Looking Glass:1"));
		}

		[Fact]
		public void IObjectOIDParseServiceTypeFailTest()
		{
			MultiParser<IObjectOID> oidParser = new([new BookObjectOIDParser(), new SpecialObjectOIDParser(), new DiscordObjectOIDParser()]);
			Action act = () => oidParser.Parse("We are never using this as a service type lol:google.com:appleseed");
			act.Should().Throw<ArgumentException>().WithMessage("The MultiParser was not able to find any IObjectOID that it could parse. (Parameter 'input')");
		}
		[Fact]
		public void IObjectOIDParseSpecialTest()
		{
			MultiParser<IObjectOID> oidParser = new([new BookObjectOIDParser(), new SpecialObjectOIDParser(), new DiscordObjectOIDParser()]);
			IObjectOID OID = oidParser.Parse("Special:_:PermissionDenied");
			OID.Should().BeOfType<SpecialObjectOID>();
			OID.Should().BeEquivalentTo(SpecialObjectOID.Parse("Special:_:PermissionDenied"));
		}
	}
}
