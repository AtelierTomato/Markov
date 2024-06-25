using AtelierTomato.Markov.Data.Model;
using AtelierTomato.Markov.Data.Model.ObjectOID;
using FluentAssertions;

namespace AtelierTomato.Markov.Data.Test
{
	public class IObjectOIDTest
	{
		[Fact]
		public void DiscordIObjectOIDParseTest()
		{
			IObjectOID OID = IObjectOIDUtil.Parse("Discord:discord.com:1253189664655806606:1253189664655806610");
			OID.Should().BeEquivalentTo(DiscordObjectOID.Parse("Discord:discord.com:1253189664655806606:1253189664655806610"));
		}
		[Fact]
		public void BookObjectOIDParseTest()
		{
			IObjectOID OID = IObjectOIDUtil.Parse("Book:_:Alice in Wonderland:Through the Looking Glass:1");
			OID.Should().BeEquivalentTo(BookObjectOID.Parse("Book:_:Alice in Wonderland:Through the Looking Glass:1"));
		}

		[Fact]
		public void IObjectOIDParseServiceTypeFailTest()
		{
			var exception = Assert.Throws<ArgumentException>(() => IObjectOIDUtil.Parse("We are never using this as a service type lol:google.com:appleseed"));
			Assert.Equal("The ServiceType was not able to be parsed from the given OID.", exception.Message);
		}
		[Fact]
		public void IObjectOIDParseInvalidTest()
		{
			var exception = Assert.Throws<ArgumentException>(() => IObjectOIDUtil.Parse("Invalid:google.com:appleseed"));
			Assert.Equal("The IObjectOID given is of ServiceType Invalid, which is not a valid ServiceType.", exception.Message);
		}
	}
}
