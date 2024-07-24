using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Model.ObjectOID;
using AtelierTomato.Markov.Storage;
using FluentAssertions;

namespace AtelierTomato.Markov.Core.Test
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
		public async void SentenceRangeAddTest()
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
			await sentenceAccess.WriteSentenceRange(sentenceStorage);
			sentenceAccess.SentenceStorage.Count.Should().Be(sentenceStorage.Count);
			sentenceAccess.SentenceStorage.Should().Contain(sentenceStorage);
		}

		[Fact]
		public async void SentenceDeleteByOIDTest()
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
			await sentenceAccess.WriteSentenceRange(sentenceStorage);
			await sentenceAccess.DeleteSentenceRange(new SentenceFilter(DiscordObjectOID.Parse("Discord:discord.com:1:2"), null), null);
			sentenceAccess.SentenceStorage.Should().HaveCount(2);
			sentenceAccess.SentenceStorage.Should().Contain([sentenceStorage[0], sentenceStorage[1]]);
			sentenceAccess.SentenceStorage.Should().NotContain([sentenceStorage[2], sentenceStorage[3]]);
		}
		[Fact]
		public async void SentenceDeleteByAuthorOIDTest()
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
			await sentenceAccess.WriteSentenceRange(sentenceStorage);
			await sentenceAccess.DeleteSentenceRange(new SentenceFilter(null, AuthorOID.Parse("Discord:discord.com:1")), null);
			sentenceAccess.SentenceStorage.Should().HaveCount(2);
			sentenceAccess.SentenceStorage.Should().Contain([sentenceStorage[1], sentenceStorage[3]]);
			sentenceAccess.SentenceStorage.Should().NotContain([sentenceStorage[0], sentenceStorage[2]]);
		}
		[Fact]
		public async void SentenceDeleteBySearchStringTest()
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
			await sentenceAccess.WriteSentenceRange(sentenceStorage);
			await sentenceAccess.DeleteSentenceRange(new SentenceFilter(null, null), "sentence");
			sentenceAccess.SentenceStorage.Should().ContainSingle();
			sentenceAccess.SentenceStorage.Should().Contain(sentenceStorage[1]);
			sentenceAccess.SentenceStorage.Should().NotContain([sentenceStorage[0], sentenceStorage[2], sentenceStorage[3]]);
		}
		[Fact]
		public async void SentenceDeleteByObjectAndAuthorTest()
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
			await sentenceAccess.WriteSentenceRange(sentenceStorage);
			await sentenceAccess.DeleteSentenceRange(new SentenceFilter(DiscordObjectOID.Parse("Discord:discord.com:1:1"), AuthorOID.Parse("Discord:discord.com:1")), null);
			sentenceAccess.SentenceStorage.Should().HaveCount(3);
			sentenceAccess.SentenceStorage.Should().Contain([sentenceStorage[1], sentenceStorage[2], sentenceStorage[3]]);
			sentenceAccess.SentenceStorage.Should().NotContain([sentenceStorage[0], sentenceStorage[4]]);
		}
		[Fact]
		public async void SentenceDeleteByAuthorAndSearchTermTest()
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
			await sentenceAccess.WriteSentenceRange(sentenceStorage);
			await sentenceAccess.DeleteSentenceRange(new SentenceFilter(null, AuthorOID.Parse("Discord:discord.com:1")), "sentence");
			sentenceAccess.SentenceStorage.Should().HaveCount(3);
			sentenceAccess.SentenceStorage.Should().Contain([sentenceStorage[1], sentenceStorage[3], sentenceStorage[4]]);
			sentenceAccess.SentenceStorage.Should().NotContain([sentenceStorage[0], sentenceStorage[2]]);
		}
		[Fact]
		public async void SentenceDeleteByObjectAndSearchTermTest()
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
			await sentenceAccess.WriteSentenceRange(sentenceStorage);
			await sentenceAccess.DeleteSentenceRange(new SentenceFilter(DiscordObjectOID.Parse("Discord:discord.com:1:1"), null), "so");
			sentenceAccess.SentenceStorage.Should().HaveCount(3);
			sentenceAccess.SentenceStorage.Should().Contain([sentenceStorage[1], sentenceStorage[2], sentenceStorage[3]]);
			sentenceAccess.SentenceStorage.Should().NotContain([sentenceStorage[0], sentenceStorage[4]]);
		}
		[Fact]
		public async void SentenceDeleteByFullFilterTermTest()
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
			await sentenceAccess.WriteSentenceRange(sentenceStorage);
			await sentenceAccess.DeleteSentenceRange(new SentenceFilter(DiscordObjectOID.Parse("Discord:discord.com:1:1"), AuthorOID.Parse("Discord:discord.com:2")), "so");
			sentenceAccess.SentenceStorage.Should().HaveCount(4);
			sentenceAccess.SentenceStorage.Should().Contain([sentenceStorage[0], sentenceStorage[1], sentenceStorage[2], sentenceStorage[3]]);
			sentenceAccess.SentenceStorage.Should().NotContain(sentenceStorage[4]);
		}
		[Fact]
		public async void SentenceDeleteAllTest()
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
			await sentenceAccess.WriteSentenceRange(sentenceStorage);
			Func<Task> act = async () => await sentenceAccess.DeleteSentenceRange(new SentenceFilter(null, null), null);
			await act.Should().ThrowAsync<ArgumentException>().WithMessage("You cannot delete all sentences from the database through this command, at least one part of the filter must have a value. (Parameter 'filter')");
		}
		[Fact]
		public async void SentenceSearchByOIDTest()
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
			await sentenceAccess.WriteSentenceRange(sentenceStorage);
			IEnumerable<Sentence>? sentenceReturn = await sentenceAccess.ReadSentenceRange(new SentenceFilter(DiscordObjectOID.Parse("Discord:discord.com:1:2"), null), null);

			sentenceReturn.Should().NotBeNull();
			sentenceReturn.Should().HaveCount(2);
			sentenceReturn.Should().Contain([sentenceStorage[2], sentenceStorage[3]]);
			sentenceReturn.Should().NotContain([sentenceStorage[0], sentenceStorage[1]]);
		}
		[Fact]
		public async void SentenceSearchByAuthorOIDTest()
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
			await sentenceAccess.WriteSentenceRange(sentenceStorage);
			IEnumerable<Sentence>? sentenceReturn = await sentenceAccess.ReadSentenceRange(new SentenceFilter(null, AuthorOID.Parse("Discord:discord.com:1")), null);

			sentenceReturn.Should().NotBeNull();
			sentenceReturn.Should().HaveCount(2);
			sentenceReturn.Should().Contain([sentenceStorage[0], sentenceStorage[2]]);
			sentenceReturn.Should().NotContain([sentenceStorage[1], sentenceStorage[3]]);
		}
		[Fact]
		public async void SentenceSearchBySearchStringTest()
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
			await sentenceAccess.WriteSentenceRange(sentenceStorage);
			IEnumerable<Sentence>? sentenceReturn = await sentenceAccess.ReadSentenceRange(new SentenceFilter(null, null), "sentence");

			sentenceReturn.Should().NotBeNull();
			sentenceReturn.Should().HaveCount(3);
			sentenceReturn.Should().Contain([sentenceStorage[0], sentenceStorage[2], sentenceStorage[3]]);
			sentenceReturn.Should().NotContain(sentenceStorage[1]);
		}
		[Fact]
		public async void SentenceSearchByObjectAndAuthorTest()
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
			await sentenceAccess.WriteSentenceRange(sentenceStorage);
			IEnumerable<Sentence>? sentenceReturn = await sentenceAccess.ReadSentenceRange(new SentenceFilter(DiscordObjectOID.Parse("Discord:discord.com:1:1"), AuthorOID.Parse("Discord:discord.com:1")), null);

			sentenceReturn.Should().NotBeNull();
			sentenceReturn.Should().HaveCount(2);
			sentenceReturn.Should().Contain([sentenceStorage[0], sentenceStorage[4]]);
			sentenceReturn.Should().NotContain([sentenceStorage[1], sentenceStorage[2], sentenceStorage[3]]);
		}
		[Fact]
		public async void SentenceReadByAuthorAndSearchTermTest()
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
			await sentenceAccess.WriteSentenceRange(sentenceStorage);
			IEnumerable<Sentence>? sentenceReturn = await sentenceAccess.ReadSentenceRange(new SentenceFilter(null, AuthorOID.Parse("Discord:discord.com:1")), "sentence");

			sentenceReturn.Should().NotBeNull();
			sentenceReturn.Should().HaveCount(2);
			sentenceReturn.Should().Contain([sentenceStorage[0], sentenceStorage[2]]);
			sentenceReturn.Should().NotContain([sentenceStorage[1], sentenceStorage[3], sentenceStorage[4]]);
		}
		[Fact]
		public async void SentenceSearchByObjectAndSearchTermTest()
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
			await sentenceAccess.WriteSentenceRange(sentenceStorage);
			IEnumerable<Sentence>? sentenceReturn = await sentenceAccess.ReadSentenceRange(new SentenceFilter(DiscordObjectOID.Parse("Discord:discord.com:1:1"), null), "so");

			sentenceReturn.Should().NotBeNull();
			sentenceReturn.Should().HaveCount(2);
			sentenceReturn.Should().Contain([sentenceStorage[0], sentenceStorage[4]]);
			sentenceReturn.Should().NotContain([sentenceStorage[1], sentenceStorage[2], sentenceStorage[3]]);
		}
		[Fact]
		public async void SentenceSearchByFullFilterTermTest()
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
			await sentenceAccess.WriteSentenceRange(sentenceStorage);
			IEnumerable<Sentence>? sentenceReturn = await sentenceAccess.ReadSentenceRange(new SentenceFilter(DiscordObjectOID.Parse("Discord:discord.com:1:1"), AuthorOID.Parse("Discord:discord.com:2")), "so");

			sentenceReturn.Should().NotBeNull();
			sentenceReturn.Should().ContainSingle();
			sentenceReturn.Should().Contain(sentenceStorage[4]);
			sentenceReturn.Should().NotContain([sentenceStorage[0], sentenceStorage[1], sentenceStorage[2], sentenceStorage[3]]);
		}

	}
}
