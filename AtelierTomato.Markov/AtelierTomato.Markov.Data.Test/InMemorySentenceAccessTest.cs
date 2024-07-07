using AtelierTomato.Markov.Data.Model;
using AtelierTomato.Markov.Data.Model.ObjectOID;
using FluentAssertions;

namespace AtelierTomato.Markov.Data.Test
{
	public class InMemorySentenceAccessTest
	{
		[Fact]
		public void SentenceAddTest()
		{
			Sentence sentence = new(
				DiscordObjectOID.Parse("Discord:discord.com:1253189664655806606:1253189664655806610:1253270827257036801:1254631007395643422:1254631136596852797:1"),
				AuthorOID.Parse("Discord:discord.com:142781100152848384"),
				DateTimeOffset.Now,
				"lol this sentence is so cool"
			);
			InMemorySentenceAccess sentenceAccess = new();
			sentenceAccess.WriteSentence(sentence);
			sentenceAccess.SentenceRange.Should().Contain(sentence);
		}
		[Fact]
		public void SentenceRangeAddTest()
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
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"wow i do not like to be in a list."
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:1"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"wow this sentence is so bad"
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:2"),
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"oh this sentence is not cool"
				),
			];
			InMemorySentenceAccess sentenceAccess = new();
			sentenceAccess.WriteSentenceRange(sentenceRange);
			sentenceAccess.SentenceRange.Should().Contain(sentenceRange);
		}

		[Fact]
		public void SentenceDeleteByOIDTest()
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
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"wow i do not like to be in a list."
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:1"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"wow this sentence is so bad"
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:2"),
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"oh this sentence is not cool"
				),
			];
			InMemorySentenceAccess sentenceAccess = new();
			sentenceAccess.WriteSentenceRange(sentenceRange);
			sentenceAccess.DeleteSentenceRange(new SentenceFilter(DiscordObjectOID.Parse("Discord:discord.com:1:2"), null, null));
			sentenceAccess.SentenceRange.Should().Contain([sentenceRange[0], sentenceRange[1]]);
			sentenceAccess.SentenceRange.Should().NotContain([sentenceRange[2], sentenceRange[3]]);
		}
		[Fact]
		public void SentenceDeleteByAuthorOIDTest()
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
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"wow i do not like to be in a list."
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:1"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"wow this sentence is so bad"
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:2"),
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"oh this sentence is not cool"
				),
			];
			InMemorySentenceAccess sentenceAccess = new();
			sentenceAccess.WriteSentenceRange(sentenceRange);
			sentenceAccess.DeleteSentenceRange(new SentenceFilter(null, AuthorOID.Parse("Discord:discord.com:1"), null));
			sentenceAccess.SentenceRange.Should().Contain([sentenceRange[1], sentenceRange[3]]);
			sentenceAccess.SentenceRange.Should().NotContain([sentenceRange[0], sentenceRange[2]]);
		}
		[Fact]
		public void SentenceDeleteBySearchStringTest()
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
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"wow i do not like to be in a list."
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:1"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"wow this sentence is so bad"
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:2"),
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"oh this sentence is not cool"
				),
			];
			InMemorySentenceAccess sentenceAccess = new();
			sentenceAccess.WriteSentenceRange(sentenceRange);
			sentenceAccess.DeleteSentenceRange(new SentenceFilter(null, null, "sentence"));
			sentenceAccess.SentenceRange.Should().Contain(sentenceRange[1]);
			sentenceAccess.SentenceRange.Should().NotContain([sentenceRange[0], sentenceRange[2], sentenceRange[3]]);
		}
		[Fact]
		public void SentenceDeleteByObjectAndAuthorTest()
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
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"wow i do not like to be in a list."
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:1"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"wow this sentence is so bad"
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:2"),
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"oh this sentence is not cool"
				),
				new (
					DiscordObjectOID.Parse("Discord:discord.com:1:1:1:1:1:3"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"i don 't want to be in a database so i 'm happy"
				)
			];
			InMemorySentenceAccess sentenceAccess = new();
			sentenceAccess.WriteSentenceRange(sentenceRange);
			sentenceAccess.DeleteSentenceRange(new SentenceFilter(DiscordObjectOID.Parse("Discord:discord.com:1:1"), AuthorOID.Parse("Discord:discord.com:1"), null));
			sentenceAccess.SentenceRange.Should().Contain([sentenceRange[1], sentenceRange[2], sentenceRange[3]]);
			sentenceAccess.SentenceRange.Should().NotContain([sentenceRange[0], sentenceRange[4]]);
		}
		[Fact]
		public void SentenceDeleteByAuthorAndSearchTermTest()
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
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"wow i do not like to be in a list."
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:1"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"wow this sentence is so bad"
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:2"),
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"oh this sentence is not cool"
				),
				new (
					DiscordObjectOID.Parse("Discord:discord.com:1:1:1:1:1:3"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"i don 't want to be in a database so i 'm happy"
				)
			];
			InMemorySentenceAccess sentenceAccess = new();
			sentenceAccess.WriteSentenceRange(sentenceRange);
			sentenceAccess.DeleteSentenceRange(new SentenceFilter(null, AuthorOID.Parse("Discord:discord.com:1"), "sentence"));
			sentenceAccess.SentenceRange.Should().Contain([sentenceRange[1], sentenceRange[3], sentenceRange[4]]);
			sentenceAccess.SentenceRange.Should().NotContain([sentenceRange[0], sentenceRange[2]]);
		}
		[Fact]
		public void SentenceDeleteByObjectAndSearchTermTest()
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
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"wow i do not like to be in a list."
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:1"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"wow this sentence is so bad"
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:2"),
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"oh this sentence is not cool"
				),
				new (
					DiscordObjectOID.Parse("Discord:discord.com:1:1:1:1:1:3"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"i don 't want to be in a database so i 'm happy"
				)
			];
			InMemorySentenceAccess sentenceAccess = new();
			sentenceAccess.WriteSentenceRange(sentenceRange);
			sentenceAccess.DeleteSentenceRange(new SentenceFilter(DiscordObjectOID.Parse("Discord:discord.com:1:1"), null, "so"));
			sentenceAccess.SentenceRange.Should().Contain([sentenceRange[1], sentenceRange[2], sentenceRange[3]]);
			sentenceAccess.SentenceRange.Should().NotContain([sentenceRange[0], sentenceRange[4]]);
		}
		[Fact]
		public void SentenceDeleteByFullFilterTermTest()
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
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"wow i do not like to be in a list."
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:1"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"wow this sentence is so bad"
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:2"),
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"oh this sentence is not cool"
				),
				new (
					DiscordObjectOID.Parse("Discord:discord.com:1:1:1:1:1:3"),
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"i don 't want to be in a database so i 'm happy"
				)
			];
			InMemorySentenceAccess sentenceAccess = new();
			sentenceAccess.WriteSentenceRange(sentenceRange);
			sentenceAccess.DeleteSentenceRange(new SentenceFilter(DiscordObjectOID.Parse("Discord:discord.com:1:1"), AuthorOID.Parse("Discord:discord.com:2"), "so"));
			sentenceAccess.SentenceRange.Should().Contain([sentenceRange[0], sentenceRange[1], sentenceRange[2], sentenceRange[3]]);
			sentenceAccess.SentenceRange.Should().NotContain(sentenceRange[4]);
		}

		[Fact]
		public void SentenceSearchByOIDTest()
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
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"wow i do not like to be in a list."
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:1"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"wow this sentence is so bad"
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:2"),
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"oh this sentence is not cool"
				),
			];
			InMemorySentenceAccess sentenceAccess = new();
			sentenceAccess.WriteSentenceRange(sentenceRange);
			IEnumerable<Sentence>? sentenceReturn = sentenceAccess.ReadSentenceRange(new SentenceFilter(DiscordObjectOID.Parse("Discord:discord.com:1:2"), null, null))?.Result ?? null;

			sentenceReturn.Should().NotBeNull();
			sentenceReturn.Should().Contain([sentenceRange[2], sentenceRange[3]]);
			sentenceReturn.Should().NotContain([sentenceRange[0], sentenceRange[1]]);
		}
		[Fact]
		public void SentenceSearchByAuthorOIDTest()
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
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"wow i do not like to be in a list."
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:1"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"wow this sentence is so bad"
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:2"),
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"oh this sentence is not cool"
				),
			];
			InMemorySentenceAccess sentenceAccess = new();
			sentenceAccess.WriteSentenceRange(sentenceRange);
			IEnumerable<Sentence>? sentenceReturn = sentenceAccess.ReadSentenceRange(new SentenceFilter(null, AuthorOID.Parse("Discord:discord.com:1"), null))?.Result ?? null;

			sentenceReturn.Should().NotBeNull();
			sentenceReturn.Should().Contain([sentenceRange[0], sentenceRange[2]]);
			sentenceReturn.Should().NotContain([sentenceRange[1], sentenceRange[3]]);
		}
		[Fact]
		public void SentenceSearchBySearchStringTest()
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
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"wow i do not like to be in a list."
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:1"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"wow this sentence is so bad"
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:2"),
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"oh this sentence is not cool"
				),
			];
			InMemorySentenceAccess sentenceAccess = new();
			sentenceAccess.WriteSentenceRange(sentenceRange);
			IEnumerable<Sentence>? sentenceReturn = sentenceAccess.ReadSentenceRange(new SentenceFilter(null, null, "sentence"))?.Result ?? null;

			sentenceReturn.Should().NotBeNull();
			sentenceReturn.Should().Contain([sentenceRange[0], sentenceRange[2], sentenceRange[3]]);
			sentenceReturn.Should().NotContain(sentenceRange[1]);
		}
		[Fact]
		public void SentenceSearchByObjectAndAuthorTest()
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
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"wow i do not like to be in a list."
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:1"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"wow this sentence is so bad"
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:2"),
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"oh this sentence is not cool"
				),
				new (
					DiscordObjectOID.Parse("Discord:discord.com:1:1:1:1:1:3"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"i don 't want to be in a database so i 'm happy"
				)
			];
			InMemorySentenceAccess sentenceAccess = new();
			sentenceAccess.WriteSentenceRange(sentenceRange);
			IEnumerable<Sentence>? sentenceReturn = sentenceAccess.ReadSentenceRange(new SentenceFilter(DiscordObjectOID.Parse("Discord:discord.com:1:1"), AuthorOID.Parse("Discord:discord.com:1"), null))?.Result ?? null;

			sentenceReturn.Should().NotBeNull();
			sentenceReturn.Should().Contain([sentenceRange[0], sentenceRange[4]]);
			sentenceReturn.Should().NotContain([sentenceRange[1], sentenceRange[2], sentenceRange[3]]);
		}
		[Fact]
		public void SentenceReadByAuthorAndSearchTermTest()
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
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"wow i do not like to be in a list."
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:1"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"wow this sentence is so bad"
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:2"),
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"oh this sentence is not cool"
				),
				new (
					DiscordObjectOID.Parse("Discord:discord.com:1:1:1:1:1:3"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"i don 't want to be in a database so i 'm happy"
				)
			];
			InMemorySentenceAccess sentenceAccess = new();
			sentenceAccess.WriteSentenceRange(sentenceRange);
			IEnumerable<Sentence>? sentenceReturn = sentenceAccess.ReadSentenceRange(new SentenceFilter(null, AuthorOID.Parse("Discord:discord.com:1"), "sentence"))?.Result ?? null;

			sentenceReturn.Should().NotBeNull();
			sentenceReturn.Should().Contain([sentenceRange[0], sentenceRange[2]]);
			sentenceReturn.Should().NotContain([sentenceRange[1], sentenceRange[3], sentenceRange[4]]);
		}
		[Fact]
		public void SentenceSearchByObjectAndSearchTermTest()
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
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"wow i do not like to be in a list."
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:1"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"wow this sentence is so bad"
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:2"),
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"oh this sentence is not cool"
				),
				new (
					DiscordObjectOID.Parse("Discord:discord.com:1:1:1:1:1:3"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"i don 't want to be in a database so i 'm happy"
				)
			];
			InMemorySentenceAccess sentenceAccess = new();
			sentenceAccess.WriteSentenceRange(sentenceRange);
			IEnumerable<Sentence>? sentenceReturn = sentenceAccess.ReadSentenceRange(new SentenceFilter(DiscordObjectOID.Parse("Discord:discord.com:1:1"), null, "so"))?.Result ?? null;

			sentenceReturn.Should().NotBeNull();
			sentenceReturn.Should().Contain([sentenceRange[0], sentenceRange[4]]);
			sentenceReturn.Should().NotContain([sentenceRange[1], sentenceRange[2], sentenceRange[3]]);
		}
		[Fact]
		public void SentenceSearchByFullFilterTermTest()
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
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"wow i do not like to be in a list."
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:1"),
					AuthorOID.Parse("Discord:discord.com:1"),
					DateTimeOffset.Now,
					"wow this sentence is so bad"
				),
				new(
					DiscordObjectOID.Parse("Discord:discord.com:1:2:1:1:1:2"),
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"oh this sentence is not cool"
				),
				new (
					DiscordObjectOID.Parse("Discord:discord.com:1:1:1:1:1:3"),
					AuthorOID.Parse("Discord:discord.com:2"),
					DateTimeOffset.Now,
					"i don 't want to be in a database so i 'm happy"
				)
			];
			InMemorySentenceAccess sentenceAccess = new();
			sentenceAccess.WriteSentenceRange(sentenceRange);
			IEnumerable<Sentence>? sentenceReturn = sentenceAccess.ReadSentenceRange(new SentenceFilter(DiscordObjectOID.Parse("Discord:discord.com:1:1"), AuthorOID.Parse("Discord:discord.com:2"), "so"))?.Result ?? null;

			sentenceReturn.Should().NotBeNull();
			sentenceReturn.Should().Contain(sentenceRange[4]);
			sentenceReturn.Should().NotContain([sentenceRange[0], sentenceRange[1], sentenceRange[2], sentenceRange[3]]);
		}

	}
}
