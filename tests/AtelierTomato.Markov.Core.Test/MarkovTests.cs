using AtelierTomato.Markov.Core.Generation;
using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Model.ObjectOID;
using AtelierTomato.Markov.Storage;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace AtelierTomato.Markov.Core.Test
{
	public class MarkovTests
	{
		[Fact]
		public void ConstructorTest()
		{
			var options = Options.Create(new MarkovChainOptions { });
			var sentenceAccess = Mock.Of<ISentenceAccess>();

			var target = new MarkovChain(sentenceAccess, options);

			target.Should().NotBeNull().And.BeOfType<MarkovChain>();
		}

		[Fact]
		public async Task DatabaseFailureEmpty()
		{
			var options = Options.Create(new MarkovChainOptions { });
			var sentenceAccess = Mock.Of<ISentenceAccess>();
			var filter = new SentenceFilter([], []);

			var target = new MarkovChain(sentenceAccess, options);
			var result = await target.Generate(filter);
			result.Should().Be(string.Empty);
		}

		[Fact]
		public async Task DatabaseFailureNull()
		{
			var options = Options.Create(new MarkovChainOptions { });
			var sentenceAccess = Mock.Of<ISentenceAccess>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
			var filter = new SentenceFilter(null, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

			var target = new MarkovChain(sentenceAccess, options);
			var result = await target.Generate(filter);
			result.Should().Be(string.Empty);
		}

		[Fact]
		public async Task MarkovChainTestWithFirst()
		{
			List<Sentence> sentenceRange = [
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:1:1:1:1:1"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"lol this sentence is so cool"
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:1:1:1:1:2"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"this is my new sentence"
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:1:1:1:1:3"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"is my computer on or not ?"
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:1:1:1:1:4"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"my head hurts so bad"
				),
			];
			InMemorySentenceAccess sentenceAccess = new();
			await sentenceAccess.WriteSentenceRange(sentenceRange);
			MarkovChain generator = new(sentenceAccess, Options.Create(new MarkovChainOptions { }));
			var result = await generator.Generate(new SentenceFilter([], []), null, "lol");
			result.Should().Be("lol this is my head");
		}
		[Fact]
		public async Task MarkovChainTestWithoutFirst()
		{
			List<Sentence> sentenceRange = [
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:1:1:1:1:1"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"look at this photograph"
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:1:1:1:1:2"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"look at this apple"
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:1:1:1:1:3"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"look at this thing"
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:1:1:1:1:4"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"look at this here"
				),
			];
			InMemorySentenceAccess sentenceAccess = new();
			await sentenceAccess.WriteSentenceRange(sentenceRange);
			MarkovChain generator = new(sentenceAccess, Options.Create(new MarkovChainOptions { }));
			var result = await generator.Generate(new SentenceFilter([], []));
			result.Should().StartWith("look at this");
			result.Should().NotBe("look at this");
		}
	}
}
