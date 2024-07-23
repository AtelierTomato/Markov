using AtelierTomato.Markov.Core.Model.ObjectOID;
using FluentAssertions;

namespace AtelierTomato.Markov.Core.Test
{
	public class DiscordObjectOIDTest
	{
		[Fact]
		public void DiscordInstanceToStringTest()
		{
			DiscordObjectOID discordMessage = DiscordObjectOID.ForInstance("discord.com");
			discordMessage.ToString().Should().Be("Discord:discord.com");
		}
		[Fact]
		public void DiscordInstanceToStringEscapeTest()
		{
			DiscordObjectOID discordMessage = DiscordObjectOID.ForInstance("di^scord:com");
			discordMessage.ToString().Should().Be("Discord:di^^scord^:com");
		}
		[Fact]
		public void DiscordServerToStringTest()
		{
			DiscordObjectOID discordMessage = DiscordObjectOID.ForServer("discord.com", 1253189664655806606);
			discordMessage.ToString().Should().Be("Discord:discord.com:1253189664655806606");
		}
		[Fact]
		public void DiscordCategoryToStringTest()
		{
			DiscordObjectOID discordMessage = DiscordObjectOID.ForCategory("discord.com", 1253189664655806606, 1253189664655806610);
			discordMessage.ToString().Should().Be("Discord:discord.com:1253189664655806606:1253189664655806610");
		}
		[Fact]
		public void DiscordChannelToStringTest()
		{
			DiscordObjectOID discordMessage = DiscordObjectOID.ForChannel("discord.com", 1253189664655806606, 1253189664655806610, 1253270827257036801);
			discordMessage.ToString().Should().Be("Discord:discord.com:1253189664655806606:1253189664655806610:1253270827257036801");
		}
		[Fact]
		public void DiscordThreadToStringTest()
		{
			DiscordObjectOID discordMessage = DiscordObjectOID.ForThread("discord.com", 1253189664655806606, 1253189664655806610, 1253270827257036801, 1254631007395643422);
			discordMessage.ToString().Should().Be("Discord:discord.com:1253189664655806606:1253189664655806610:1253270827257036801:1254631007395643422");
		}
		[Fact]
		public void DiscordMessageToStringTest()
		{
			DiscordObjectOID discordMessage = DiscordObjectOID.ForMessage("discord.com", 1253189664655806606, 1253189664655806610, 1253270827257036801, 1254631007395643422, 1254631136596852797);
			discordMessage.ToString().Should().Be("Discord:discord.com:1253189664655806606:1253189664655806610:1253270827257036801:1254631007395643422:1254631136596852797");
		}
		[Fact]
		public void DiscordSentenceToStringTest()
		{
			DiscordObjectOID discordMessage = DiscordObjectOID.ForSentence("discord.com", 1253189664655806606, 1253189664655806610, 1253270827257036801, 1254631007395643422, 1254631136596852797, 1);
			discordMessage.ToString().Should().Be("Discord:discord.com:1253189664655806606:1253189664655806610:1253270827257036801:1254631007395643422:1254631136596852797:1");
		}

		[Fact]
		public void DiscordInstanceParseTest()
		{
			DiscordObjectOID discordMessage = DiscordObjectOID.Parse("Discord:discord.com");
			discordMessage.Should().BeEquivalentTo(DiscordObjectOID.ForInstance("discord.com"));
		}
		[Fact]
		public void DiscordServerParseTest()
		{
			DiscordObjectOID discordMessage = DiscordObjectOID.Parse("Discord:discord.com:1253189664655806606");
			discordMessage.Should().BeEquivalentTo(DiscordObjectOID.ForServer("discord.com", 1253189664655806606));
		}
		[Fact]
		public void DiscordCategoryParseTest()
		{
			DiscordObjectOID discordMessage = DiscordObjectOID.Parse("Discord:discord.com:1253189664655806606:1253189664655806610");
			discordMessage.Should().BeEquivalentTo(DiscordObjectOID.ForCategory("discord.com", 1253189664655806606, 1253189664655806610));
		}
		[Fact]
		public void DiscordChannelParseTest()
		{
			DiscordObjectOID discordMessage = DiscordObjectOID.Parse("Discord:discord.com:1253189664655806606:1253189664655806610:1253270827257036801");
			discordMessage.Should().BeEquivalentTo(DiscordObjectOID.ForChannel("discord.com", 1253189664655806606, 1253189664655806610, 1253270827257036801));
		}
		[Fact]
		public void DiscordThreadParseTest()
		{
			DiscordObjectOID discordMessage = DiscordObjectOID.Parse("Discord:discord.com:1253189664655806606:1253189664655806610:1253270827257036801:1254631007395643422");
			discordMessage.Should().BeEquivalentTo(DiscordObjectOID.ForThread("discord.com", 1253189664655806606, 1253189664655806610, 1253270827257036801, 1254631007395643422));
		}
		[Fact]
		public void DiscordMessageParseTest()
		{
			DiscordObjectOID discordMessage = DiscordObjectOID.Parse("Discord:discord.com:1253189664655806606:1253189664655806610:1253270827257036801:1254631007395643422:1254633446295207966");
			discordMessage.Should().BeEquivalentTo(DiscordObjectOID.ForMessage("discord.com", 1253189664655806606, 1253189664655806610, 1253270827257036801, 1254631007395643422, 1254633446295207966));
		}
		[Fact]
		public void DiscordSentenceParseTest()
		{
			DiscordObjectOID discordMessage = DiscordObjectOID.Parse("Discord:discord.com:1253189664655806606:1253189664655806610:1253270827257036801:1254631007395643422:1254633446295207966:2");
			discordMessage.Should().BeEquivalentTo(DiscordObjectOID.ForSentence("discord.com", 1253189664655806606, 1253189664655806610, 1253270827257036801, 1254631007395643422, 1254633446295207966, 2));
		}

		[Fact]
		public void DiscordParseTooLongTest()
		{
			Action act = () => DiscordObjectOID.Parse("Discord:discord.com:1253189664655806606:1253189664655806610:1253270827257036801:1254631007395643422:1254633446295207966:2:Appleseed");
			act.Should().Throw<ArgumentException>().WithMessage("The OID given is not a valid DiscordObjectOID. (Parameter 'OID')");
		}
		[Fact]
		public void BookParseEmptyTest()
		{
			Action act = () => DiscordObjectOID.Parse(String.Empty);
			act.Should().Throw<ArgumentException>().WithMessage("The OID given is empty. (Parameter 'OID')");
		}
		[Fact]
		public void DiscordParseNotADiscordTest()
		{
			Action act = () => DiscordObjectOID.Parse("Invalid:1:Appleseed:???:4:Greg");
			act.Should().Throw<ArgumentException>().WithMessage("The OID given is not a DiscordObjectOID, as it does not begin with Discord. (Parameter 'OID')");
		}
		[Fact]
		public void DiscordParseOnlyHasSerivceTypeTest()
		{
			Action act = () => DiscordObjectOID.Parse("Discord");
			act.Should().Throw<ArgumentException>().WithMessage("The OID given is not a valid DiscordObjectOID. (Parameter 'OID')");
		}
		[Fact]
		public void DiscordParseServerNotUlongTest()
		{
			Action act = () => DiscordObjectOID.Parse("Discord:discord.com:Atelier Tomato");
			act.Should().Throw<ArgumentException>().WithMessage("The part of the DiscordObjectOID corresponding to the server was not able to be parsed into a ulong value. (Parameter 'OID')");
		}
		[Fact]
		public void DiscordParseCategoryNotUlongTest()
		{
			Action act = () => DiscordObjectOID.Parse("Discord:discord.com:1253189664655806606:channels");
			act.Should().Throw<ArgumentException>().WithMessage("The part of the DiscordObjectOID corresponding to the category was not able to be parsed into a ulong value. (Parameter 'OID')");
		}
		[Fact]
		public void DiscordParseChannelNotUlongTest()
		{
			Action act = () => DiscordObjectOID.Parse("Discord:discord.com:1253189664655806606:1253189664655806610:programming");
			act.Should().Throw<ArgumentException>().WithMessage("The part of the DiscordObjectOID corresponding to the channel was not able to be parsed into a ulong value. (Parameter 'OID')");
		}
		[Fact]
		public void DiscordParseThreadNotUlongTest()
		{
			Action act = () => DiscordObjectOID.Parse("Discord:discord.com:1253189664655806606:1253189664655806610:1253270827257036801:test thread lol");
			act.Should().Throw<ArgumentException>().WithMessage("The part of the DiscordObjectOID corresponding to the thread was not able to be parsed into a ulong value. (Parameter 'OID')");
		}
		[Fact]
		public void DiscordParseMessageNotUlongTest()
		{
			Action act = () => DiscordObjectOID.Parse("Discord:discord.com:1253189664655806606:1253189664655806610:1253270827257036801:1254631007395643422:MyNewSentence");
			act.Should().Throw<ArgumentException>().WithMessage("The part of the DiscordObjectOID corresponding to the message was not able to be parsed into a ulong value. (Parameter 'OID')");
		}
		[Fact]
		public void DiscordParseSentenceNotIntTest()
		{
			Action act = () => DiscordObjectOID.Parse("Discord:discord.com:1253189664655806606:1253189664655806610:1253270827257036801:1254631007395643422:1254631778308849716:Four");
			act.Should().Throw<ArgumentException>().WithMessage("The part of the DiscordObjectOID corresponding to the sentence was not able to be parsed into an int value. (Parameter 'OID')");
		}
	}
}
