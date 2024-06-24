﻿using AtelierTomato.Markov.Data.Model.ObjectOID;
using FluentAssertions;

namespace AtelierTomato.Markov.Data.Test
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
		public void DiscordChannelToStringTest()
		{
			DiscordObjectOID discordMessage = DiscordObjectOID.ForChannel("discord.com", 1253189664655806606, 1253270827257036801);
			discordMessage.ToString().Should().Be("Discord:discord.com:1253189664655806606:1253270827257036801");
		}
		[Fact]
		public void DiscordMessageToStringTest()
		{
			DiscordObjectOID discordMessage = DiscordObjectOID.ForMessage("discord.com", 1253189664655806606, 1253270827257036801, 1254633446295207966);
			discordMessage.ToString().Should().Be("Discord:discord.com:1253189664655806606:1253270827257036801:1254633446295207966");
		}
		[Fact]
		public void DiscordSentenceToStringTest()
		{
			DiscordObjectOID discordMessage = DiscordObjectOID.ForSentence("discord.com", 1253189664655806606, 1253270827257036801, 1254633446295207966, 2);
			discordMessage.ToString().Should().Be("Discord:discord.com:1253189664655806606:1253270827257036801:1254633446295207966:2");
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
		public void DiscordChannelParseTest()
		{
			DiscordObjectOID discordMessage = DiscordObjectOID.Parse("Discord:discord.com:1253189664655806606:1253270827257036801");
			discordMessage.Should().BeEquivalentTo(DiscordObjectOID.ForChannel("discord.com", 1253189664655806606, 1253270827257036801));
		}
		[Fact]
		public void DiscordMessageParseTest()
		{
			DiscordObjectOID discordMessage = DiscordObjectOID.Parse("Discord:discord.com:1253189664655806606:1253270827257036801:1254633446295207966");
			discordMessage.Should().BeEquivalentTo(DiscordObjectOID.ForMessage("discord.com", 1253189664655806606, 1253270827257036801, 1254633446295207966));
		}
		[Fact]
		public void DiscordSentenceParseTest()
		{
			DiscordObjectOID discordMessage = DiscordObjectOID.Parse("Discord:discord.com:1253189664655806606:1253270827257036801:1254633446295207966:2");
			discordMessage.Should().BeEquivalentTo(DiscordObjectOID.ForSentence("discord.com", 1253189664655806606, 1253270827257036801, 1254633446295207966, 2));
		}
	}
}
