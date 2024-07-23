using AtelierTomato.Markov.Data.Generation;
using AtelierTomato.Markov.Data.Model;
using AtelierTomato.Markov.Data.Model.ObjectOID;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace AtelierTomato.Markov.Data.Test
{
	public class MarkovTests
	{
		[Fact]
		public void ConstructorTest()
		{
			var options = Options.Create(new MarkovGenerationOptions { });
			var sentenceAccess = Mock.Of<ISentenceAccess>();

			var target = new MarkovChain(sentenceAccess, options);

			target.Should().NotBeNull().And.BeOfType<MarkovChain>();
		}

		[Fact]
		public async Task DatabaseFailure()
		{
			var options = Options.Create(new MarkovGenerationOptions { });
			var sentenceAccess = Mock.Of<ISentenceAccess>();
			var filter = new SentenceFilter(null, null);


			Func<Task> action = async () =>
			{
				var target = new MarkovChain(sentenceAccess, options);

				var result = await target.Generate(filter);

			};
			await action.Should().ThrowAsync<Exception>().WithMessage("Couldn't query any messages.");
		}

		[Fact]
		public async void MarkovChainTestWithFirst()
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
			MarkovChain generator = new(sentenceAccess, Options.Create(new MarkovGenerationOptions { }));
			var result = await generator.Generate(new SentenceFilter(null, null), null, "lol");
			result.Should().Be("lol this is my head");
		}
		[Fact]
		public async void MarkovChainTestWithoutFirst()
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
			MarkovChain generator = new(sentenceAccess, Options.Create(new MarkovGenerationOptions { }));
			var result = await generator.Generate(new SentenceFilter(null, null));
			result.Should().StartWith("look at this");
			result.Should().NotBe("look at this");
		}
	}
}