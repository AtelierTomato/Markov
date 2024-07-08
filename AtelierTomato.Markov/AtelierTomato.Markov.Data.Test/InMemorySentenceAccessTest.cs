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
			sentenceAccess.SentenceStorage.Should().Contain(sentence);
		}
		[Fact]
		public void SentenceRangeAddTest()
		{
			List<Sentence> sentenceStorage = [
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
			sentenceAccess.WriteSentenceRange(sentenceStorage);
			sentenceAccess.SentenceStorage.Count.Should().Be(sentenceStorage.Count);
			sentenceAccess.SentenceStorage.Should().Contain(sentenceStorage);
		}

		[Fact]
		public void SentenceDeleteByOIDTest()
		{
			List<Sentence> sentenceStorage = [
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
			sentenceAccess.WriteSentenceRange(sentenceStorage);
			sentenceAccess.DeleteSentenceRange(new SentenceFilter(DiscordObjectOID.Parse("Discord:discord.com:1:2"), null, null));
			sentenceAccess.SentenceStorage.Should().HaveCount(2);
			sentenceAccess.SentenceStorage.Should().Contain([sentenceStorage[0], sentenceStorage[1]]);
			sentenceAccess.SentenceStorage.Should().NotContain([sentenceStorage[2], sentenceStorage[3]]);
		}
		[Fact]
		public void SentenceDeleteByAuthorOIDTest()
		{
			List<Sentence> sentenceStorage = [
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
			sentenceAccess.WriteSentenceRange(sentenceStorage);
			sentenceAccess.DeleteSentenceRange(new SentenceFilter(null, AuthorOID.Parse("Discord:discord.com:1"), null));
			sentenceAccess.SentenceStorage.Should().HaveCount(2);
			sentenceAccess.SentenceStorage.Should().Contain([sentenceStorage[1], sentenceStorage[3]]);
			sentenceAccess.SentenceStorage.Should().NotContain([sentenceStorage[0], sentenceStorage[2]]);
		}
		[Fact]
		public void SentenceDeleteBySearchStringTest()
		{
			List<Sentence> sentenceStorage = [
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
			sentenceAccess.WriteSentenceRange(sentenceStorage);
			sentenceAccess.DeleteSentenceRange(new SentenceFilter(null, null, "sentence"));
			sentenceAccess.SentenceStorage.Should().ContainSingle();
			sentenceAccess.SentenceStorage.Should().Contain(sentenceStorage[1]);
			sentenceAccess.SentenceStorage.Should().NotContain([sentenceStorage[0], sentenceStorage[2], sentenceStorage[3]]);
		}
		[Fact]
		public void SentenceDeleteByObjectAndAuthorTest()
		{
			List<Sentence> sentenceStorage = [
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
			sentenceAccess.WriteSentenceRange(sentenceStorage);
			sentenceAccess.DeleteSentenceRange(new SentenceFilter(DiscordObjectOID.Parse("Discord:discord.com:1:1"), AuthorOID.Parse("Discord:discord.com:1"), null));
			sentenceAccess.SentenceStorage.Should().HaveCount(3);
			sentenceAccess.SentenceStorage.Should().Contain([sentenceStorage[1], sentenceStorage[2], sentenceStorage[3]]);
			sentenceAccess.SentenceStorage.Should().NotContain([sentenceStorage[0], sentenceStorage[4]]);
		}
		[Fact]
		public void SentenceDeleteByAuthorAndSearchTermTest()
		{
			List<Sentence> sentenceStorage = [
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
			sentenceAccess.WriteSentenceRange(sentenceStorage);
			sentenceAccess.DeleteSentenceRange(new SentenceFilter(null, AuthorOID.Parse("Discord:discord.com:1"), "sentence"));
			sentenceAccess.SentenceStorage.Should().HaveCount(3);
			sentenceAccess.SentenceStorage.Should().Contain([sentenceStorage[1], sentenceStorage[3], sentenceStorage[4]]);
			sentenceAccess.SentenceStorage.Should().NotContain([sentenceStorage[0], sentenceStorage[2]]);
		}
		[Fact]
		public void SentenceDeleteByObjectAndSearchTermTest()
		{
			List<Sentence> sentenceStorage = [
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
			sentenceAccess.WriteSentenceRange(sentenceStorage);
			sentenceAccess.DeleteSentenceRange(new SentenceFilter(DiscordObjectOID.Parse("Discord:discord.com:1:1"), null, "so"));
			sentenceAccess.SentenceStorage.Should().HaveCount(3);
			sentenceAccess.SentenceStorage.Should().Contain([sentenceStorage[1], sentenceStorage[2], sentenceStorage[3]]);
			sentenceAccess.SentenceStorage.Should().NotContain([sentenceStorage[0], sentenceStorage[4]]);
		}
		[Fact]
		public void SentenceDeleteByFullFilterTermTest()
		{
			List<Sentence> sentenceStorage = [
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
			sentenceAccess.WriteSentenceRange(sentenceStorage);
			sentenceAccess.DeleteSentenceRange(new SentenceFilter(DiscordObjectOID.Parse("Discord:discord.com:1:1"), AuthorOID.Parse("Discord:discord.com:2"), "so"));
			sentenceAccess.SentenceStorage.Should().HaveCount(4);
			sentenceAccess.SentenceStorage.Should().Contain([sentenceStorage[0], sentenceStorage[1], sentenceStorage[2], sentenceStorage[3]]);
			sentenceAccess.SentenceStorage.Should().NotContain(sentenceStorage[4]);
		}
		[Fact]
		public void SentenceDeleteAllTest()
		{
			List<Sentence> sentenceStorage = [
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
			sentenceAccess.WriteSentenceRange(sentenceStorage);
			Action act = () => sentenceAccess.DeleteSentenceRange(new SentenceFilter(null, null, null));
			act.Should().Throw<ArgumentException>().WithMessage("You cannot delete all sentences from the database through this command, at least one part of the filter must have a value. (Parameter 'filter')");
		}
		[Fact]
		public void SentenceSearchByOIDTest()
		{
			List<Sentence> sentenceStorage = [
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
			sentenceAccess.WriteSentenceRange(sentenceStorage);
			IEnumerable<Sentence>? sentenceReturn = sentenceAccess.ReadSentenceRange(new SentenceFilter(DiscordObjectOID.Parse("Discord:discord.com:1:2"), null, null))?.Result ?? null;

			sentenceReturn.Should().NotBeNull();
			sentenceReturn.Should().HaveCount(2);
			sentenceReturn.Should().Contain([sentenceStorage[2], sentenceStorage[3]]);
			sentenceReturn.Should().NotContain([sentenceStorage[0], sentenceStorage[1]]);
		}
		[Fact]
		public void SentenceSearchByAuthorOIDTest()
		{
			List<Sentence> sentenceStorage = [
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
			sentenceAccess.WriteSentenceRange(sentenceStorage);
			IEnumerable<Sentence>? sentenceReturn = sentenceAccess.ReadSentenceRange(new SentenceFilter(null, AuthorOID.Parse("Discord:discord.com:1"), null))?.Result ?? null;

			sentenceReturn.Should().NotBeNull();
			sentenceReturn.Should().HaveCount(2);
			sentenceReturn.Should().Contain([sentenceStorage[0], sentenceStorage[2]]);
			sentenceReturn.Should().NotContain([sentenceStorage[1], sentenceStorage[3]]);
		}
		[Fact]
		public void SentenceSearchBySearchStringTest()
		{
			List<Sentence> sentenceStorage = [
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
			sentenceAccess.WriteSentenceRange(sentenceStorage);
			IEnumerable<Sentence>? sentenceReturn = sentenceAccess.ReadSentenceRange(new SentenceFilter(null, null, "sentence"))?.Result ?? null;

			sentenceReturn.Should().NotBeNull();
			sentenceReturn.Should().HaveCount(3);
			sentenceReturn.Should().Contain([sentenceStorage[0], sentenceStorage[2], sentenceStorage[3]]);
			sentenceReturn.Should().NotContain(sentenceStorage[1]);
		}
		[Fact]
		public void SentenceSearchByObjectAndAuthorTest()
		{
			List<Sentence> sentenceStorage = [
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
			sentenceAccess.WriteSentenceRange(sentenceStorage);
			IEnumerable<Sentence>? sentenceReturn = sentenceAccess.ReadSentenceRange(new SentenceFilter(DiscordObjectOID.Parse("Discord:discord.com:1:1"), AuthorOID.Parse("Discord:discord.com:1"), null))?.Result ?? null;

			sentenceReturn.Should().NotBeNull();
			sentenceReturn.Should().HaveCount(2);
			sentenceReturn.Should().Contain([sentenceStorage[0], sentenceStorage[4]]);
			sentenceReturn.Should().NotContain([sentenceStorage[1], sentenceStorage[2], sentenceStorage[3]]);
		}
		[Fact]
		public void SentenceReadByAuthorAndSearchTermTest()
		{
			List<Sentence> sentenceStorage = [
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
			sentenceAccess.WriteSentenceRange(sentenceStorage);
			IEnumerable<Sentence>? sentenceReturn = sentenceAccess.ReadSentenceRange(new SentenceFilter(null, AuthorOID.Parse("Discord:discord.com:1"), "sentence"))?.Result ?? null;

			sentenceReturn.Should().NotBeNull();
			sentenceReturn.Should().HaveCount(2);
			sentenceReturn.Should().Contain([sentenceStorage[0], sentenceStorage[2]]);
			sentenceReturn.Should().NotContain([sentenceStorage[1], sentenceStorage[3], sentenceStorage[4]]);
		}
		[Fact]
		public void SentenceSearchByObjectAndSearchTermTest()
		{
			List<Sentence> sentenceStorage = [
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
			sentenceAccess.WriteSentenceRange(sentenceStorage);
			IEnumerable<Sentence>? sentenceReturn = sentenceAccess.ReadSentenceRange(new SentenceFilter(DiscordObjectOID.Parse("Discord:discord.com:1:1"), null, "so"))?.Result ?? null;

			sentenceReturn.Should().NotBeNull();
			sentenceReturn.Should().HaveCount(2);
			sentenceReturn.Should().Contain([sentenceStorage[0], sentenceStorage[4]]);
			sentenceReturn.Should().NotContain([sentenceStorage[1], sentenceStorage[2], sentenceStorage[3]]);
		}
		[Fact]
		public void SentenceSearchByFullFilterTermTest()
		{
			List<Sentence> sentenceStorage = [
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
			sentenceAccess.WriteSentenceRange(sentenceStorage);
			IEnumerable<Sentence>? sentenceReturn = sentenceAccess.ReadSentenceRange(new SentenceFilter(DiscordObjectOID.Parse("Discord:discord.com:1:1"), AuthorOID.Parse("Discord:discord.com:2"), "so"))?.Result ?? null;

			sentenceReturn.Should().NotBeNull();
			sentenceReturn.Should().ContainSingle();
			sentenceReturn.Should().Contain(sentenceStorage[4]);
			sentenceReturn.Should().NotContain([sentenceStorage[0], sentenceStorage[1], sentenceStorage[2], sentenceStorage[3]]);
		}

	}
}
