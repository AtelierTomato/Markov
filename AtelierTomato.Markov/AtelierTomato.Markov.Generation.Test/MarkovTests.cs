using AtelierTomato.Markov.Data;
using AtelierTomato.Markov.Data.Model;
using AtelierTomato.Markov.Data.Model.ObjectOID;
using AtelierTomato.Markov.Data.SentenceAccess;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace AtelierTomato.Markov.Generation.Test
{
	public class MarkovTests
	{
		[Fact]
		public void ConstructorTest()
		{
			var options = Options.Create(new MarkovGenerationOptions { });
			var sentenceAccess = Mock.Of<ISentenceAccess>();

			var target = new GenerateMarkovSentence(sentenceAccess, options);

			target.Should().NotBeNull().And.BeOfType<GenerateMarkovSentence>();
		}

		[Fact]
		public async Task DatabaseFailure()
		{
			var options = Options.Create(new MarkovGenerationOptions { });
			var sentenceAccess = Mock.Of<ISentenceAccess>();
			var filter = new SentenceFilter(null, null, null);


			Func<Task> action = async () =>
			{
				var target = new GenerateMarkovSentence(sentenceAccess, options);

				var result = await target.Generate(filter);

			};
			await action.Should().ThrowAsync<Exception>().WithMessage("Couldn't query any messages.");
		}

		[Fact]
		public void MarkovChainTest()
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
			sentenceAccess.WriteSentenceRange(sentenceRange);
			GenerateMarkovSentence generator = new(sentenceAccess, Options.Create(new MarkovGenerationOptions { }));
			var result = generator.Generate(new SentenceFilter(null, null, null), "lol").Result;
			result.Should().Be("lol this is my head");
		}
	}
}