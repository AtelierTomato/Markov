using AtelierTomato.Markov.Data.Model;
using AtelierTomato.Markov.Data.Model.ObjectOID;
using AtelierTomato.Markov.Data.Model.ObjectOID.Parser;
using FluentAssertions;

namespace AtelierTomato.Markov.Data.Test
{
	public class MultiParserTest
	{
		[Fact]
		public void DiscordIObjectOIDParseTest()
		{
			MultiParser<IObjectOID> oidParser = new([new BookObjectOIDParser(), new InvalidObjectOIDParser(), new DiscordObjectOIDParser()]);
			IObjectOID OID = oidParser.Parse("Discord:discord.com:1253189664655806606:1253189664655806610");
			OID.Should().BeEquivalentTo(DiscordObjectOID.Parse("Discord:discord.com:1253189664655806606:1253189664655806610"));
		}
		[Fact]
		public void BookObjectOIDParseTest()
		{
			MultiParser<IObjectOID> oidParser = new([new BookObjectOIDParser(), new InvalidObjectOIDParser(), new DiscordObjectOIDParser()]);
			IObjectOID OID = oidParser.Parse("Book:_:Alice in Wonderland:Through the Looking Glass:1");
			OID.Should().BeEquivalentTo(BookObjectOID.Parse("Book:_:Alice in Wonderland:Through the Looking Glass:1"));
		}

		[Fact]
		public void IObjectOIDParseServiceTypeFailTest()
		{
			MultiParser<IObjectOID> oidParser = new([new BookObjectOIDParser(), new InvalidObjectOIDParser(), new DiscordObjectOIDParser()]);
			var exception = Assert.Throws<ArgumentException>(() => oidParser.Parse("We are never using this as a service type lol:google.com:appleseed"));
			Assert.Equal("The ServiceType was not able to be parsed from the given OID.", exception.Message);
		}
		[Fact]
		public void IObjectOIDParseInvalidTest()
		{
			MultiParser<IObjectOID> oidParser = new([new BookObjectOIDParser(), new InvalidObjectOIDParser(), new DiscordObjectOIDParser()]);
			var exception = Assert.Throws<ArgumentException>(() => oidParser.Parse("Invalid:google.com:appleseed"));
			Assert.Equal("The IObjectOID given is of ServiceType Invalid, which is not a valid ServiceType.", exception.Message);
		}
	}
}
