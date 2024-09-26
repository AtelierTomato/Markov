using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Model.ObjectOID;
using FluentAssertions;

namespace AtelierTomato.Markov.Storage.Test
{
	public class InMemorySentenceAccessTests
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
		public async Task SentenceRangeAddTest()
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
		public async Task SentenceDeleteByOIDTest()
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
			await sentenceAccess.DeleteSentenceRange(new SentenceFilter([DiscordObjectOID.Parse("Discord:discord.com:1:2")], []), null);
			sentenceAccess.SentenceStorage.Should().HaveCount(2);
			sentenceAccess.SentenceStorage.Should().Contain([sentenceStorage[0], sentenceStorage[1]]);
			sentenceAccess.SentenceStorage.Should().NotContain([sentenceStorage[2], sentenceStorage[3]]);
		}
		[Fact]
		public async Task SentenceDeleteByAuthorOIDTest()
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
			await sentenceAccess.DeleteSentenceRange(new SentenceFilter([], [AuthorOID.Parse("Discord:discord.com:1")]), null);
			sentenceAccess.SentenceStorage.Should().HaveCount(2);
			sentenceAccess.SentenceStorage.Should().Contain([sentenceStorage[1], sentenceStorage[3]]);
			sentenceAccess.SentenceStorage.Should().NotContain([sentenceStorage[0], sentenceStorage[2]]);
		}
		[Fact]
		public async Task SentenceDeleteBySearchStringTest()
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
			await sentenceAccess.DeleteSentenceRange(new SentenceFilter([], []), "sentence");
			sentenceAccess.SentenceStorage.Should().ContainSingle();
			sentenceAccess.SentenceStorage.Should().Contain(sentenceStorage[1]);
			sentenceAccess.SentenceStorage.Should().NotContain([sentenceStorage[0], sentenceStorage[2], sentenceStorage[3]]);
		}
		[Fact]
		public async Task SentenceDeleteByObjectAndAuthorTest()
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
			await sentenceAccess.DeleteSentenceRange(new SentenceFilter([DiscordObjectOID.Parse("Discord:discord.com:1:1")], [AuthorOID.Parse("Discord:discord.com:1")]), null);
			sentenceAccess.SentenceStorage.Should().HaveCount(3);
			sentenceAccess.SentenceStorage.Should().Contain([sentenceStorage[1], sentenceStorage[2], sentenceStorage[3]]);
			sentenceAccess.SentenceStorage.Should().NotContain([sentenceStorage[0], sentenceStorage[4]]);
		}
		[Fact]
		public async Task SentenceDeleteByAuthorAndSearchTermTest()
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
			await sentenceAccess.DeleteSentenceRange(new SentenceFilter([], [AuthorOID.Parse("Discord:discord.com:1")]), "sentence");
			sentenceAccess.SentenceStorage.Should().HaveCount(3);
			sentenceAccess.SentenceStorage.Should().Contain([sentenceStorage[1], sentenceStorage[3], sentenceStorage[4]]);
			sentenceAccess.SentenceStorage.Should().NotContain([sentenceStorage[0], sentenceStorage[2]]);
		}
		[Fact]
		public async Task SentenceDeleteByObjectAndSearchTermTest()
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
			await sentenceAccess.DeleteSentenceRange(new SentenceFilter([DiscordObjectOID.Parse("Discord:discord.com:1:1")], []), "so");
			sentenceAccess.SentenceStorage.Should().HaveCount(3);
			sentenceAccess.SentenceStorage.Should().Contain([sentenceStorage[1], sentenceStorage[2], sentenceStorage[3]]);
			sentenceAccess.SentenceStorage.Should().NotContain([sentenceStorage[0], sentenceStorage[4]]);
		}
		[Fact]
		public async Task SentenceDeleteByFullFilterTermTest()
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
			await sentenceAccess.DeleteSentenceRange(new SentenceFilter([DiscordObjectOID.Parse("Discord:discord.com:1:1")], [AuthorOID.Parse("Discord:discord.com:2")]), "so");
			sentenceAccess.SentenceStorage.Should().HaveCount(4);
			sentenceAccess.SentenceStorage.Should().Contain([sentenceStorage[0], sentenceStorage[1], sentenceStorage[2], sentenceStorage[3]]);
			sentenceAccess.SentenceStorage.Should().NotContain(sentenceStorage[4]);
		}
		[Fact]
		public async Task SentenceDeleteAllTest()
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
			Func<Task> act = async () => await sentenceAccess.DeleteSentenceRange(new SentenceFilter([], []), null);
			await act.Should().ThrowAsync<ArgumentException>().WithMessage("You cannot delete all sentences from the database through this command, at least one part of the filter must have a value. (Parameter 'filter')");
		}
		[Fact]
		public async Task SentenceSearchByOIDTest()
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
			IEnumerable<Sentence>? sentenceReturn = await sentenceAccess.ReadSentenceRange(new SentenceFilter([DiscordObjectOID.Parse("Discord:discord.com:1:2")], []), null);

			sentenceReturn.Should().NotBeNull();
			sentenceReturn.Should().HaveCount(2);
			sentenceReturn.Should().Contain([sentenceStorage[2], sentenceStorage[3]]);
			sentenceReturn.Should().NotContain([sentenceStorage[0], sentenceStorage[1]]);
		}
		[Fact]
		public async Task SentenceSearchByAuthorOIDTest()
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
			IEnumerable<Sentence>? sentenceReturn = await sentenceAccess.ReadSentenceRange(new SentenceFilter([], [AuthorOID.Parse("Discord:discord.com:1")]), null);

			sentenceReturn.Should().NotBeNull();
			sentenceReturn.Should().HaveCount(2);
			sentenceReturn.Should().Contain([sentenceStorage[0], sentenceStorage[2]]);
			sentenceReturn.Should().NotContain([sentenceStorage[1], sentenceStorage[3]]);
		}
		[Fact]
		public async Task SentenceSearchBySearchStringTest()
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
			IEnumerable<Sentence>? sentenceReturn = await sentenceAccess.ReadSentenceRange(new SentenceFilter([], []), "sentence");

			sentenceReturn.Should().NotBeNull();
			sentenceReturn.Should().HaveCount(3);
			sentenceReturn.Should().Contain([sentenceStorage[0], sentenceStorage[2], sentenceStorage[3]]);
			sentenceReturn.Should().NotContain(sentenceStorage[1]);
		}
		[Fact]
		public async Task SentenceSearchByObjectAndAuthorTest()
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
			IEnumerable<Sentence>? sentenceReturn = await sentenceAccess.ReadSentenceRange(new SentenceFilter([DiscordObjectOID.Parse("Discord:discord.com:1:1")], [AuthorOID.Parse("Discord:discord.com:1")]), null);

			sentenceReturn.Should().NotBeNull();
			sentenceReturn.Should().HaveCount(2);
			sentenceReturn.Should().Contain([sentenceStorage[0], sentenceStorage[4]]);
			sentenceReturn.Should().NotContain([sentenceStorage[1], sentenceStorage[2], sentenceStorage[3]]);
		}
		[Fact]
		public async Task SentenceReadByAuthorAndSearchTermTest()
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
			IEnumerable<Sentence>? sentenceReturn = await sentenceAccess.ReadSentenceRange(new SentenceFilter([], [AuthorOID.Parse("Discord:discord.com:1")]), "sentence");

			sentenceReturn.Should().NotBeNull();
			sentenceReturn.Should().HaveCount(2);
			sentenceReturn.Should().Contain([sentenceStorage[0], sentenceStorage[2]]);
			sentenceReturn.Should().NotContain([sentenceStorage[1], sentenceStorage[3], sentenceStorage[4]]);
		}
		[Fact]
		public async Task SentenceSearchByObjectAndSearchTermTest()
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
			IEnumerable<Sentence>? sentenceReturn = await sentenceAccess.ReadSentenceRange(new SentenceFilter([DiscordObjectOID.Parse("Discord:discord.com:1:1")], []), "so");

			sentenceReturn.Should().NotBeNull();
			sentenceReturn.Should().HaveCount(2);
			sentenceReturn.Should().Contain([sentenceStorage[0], sentenceStorage[4]]);
			sentenceReturn.Should().NotContain([sentenceStorage[1], sentenceStorage[2], sentenceStorage[3]]);
		}
		[Fact]
		public async Task SentenceSearchByFullFilterTermTest()
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
			IEnumerable<Sentence>? sentenceReturn = await sentenceAccess.ReadSentenceRange(new SentenceFilter([DiscordObjectOID.Parse("Discord:discord.com:1:1")], [AuthorOID.Parse("Discord:discord.com:2")]), "so");

			sentenceReturn.Should().NotBeNull();
			sentenceReturn.Should().ContainSingle();
			sentenceReturn.Should().Contain(sentenceStorage[4]);
			sentenceReturn.Should().NotContain([sentenceStorage[0], sentenceStorage[1], sentenceStorage[2], sentenceStorage[3]]);
		}

	}
}
