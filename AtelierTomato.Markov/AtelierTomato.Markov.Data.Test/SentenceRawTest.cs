using AtelierTomato.Markov.Data.Model;
using AtelierTomato.Markov.Data.Model.ObjectOID.Parser;
using FluentAssertions;

namespace AtelierTomato.Markov.Data.Test
{
	public class SentenceRawTest
	{
		private readonly MultiParser<IObjectOID> ObjectOIDParser = new([new InvalidObjectOIDParser(), new BookObjectOIDParser(), new DiscordObjectOIDParser()]);

		[Fact]
		public void SentenceRawFromSentenceTest()
		{
			Sentence sentence = new Sentence(
				ObjectOIDParser.Parse("Discord:discord.com:1253189664655806606:1253189664655806610:1253270827257036801:1254631007395643422:1254631136596852797:1"),
				AuthorOID.Parse("Discord:discord.com:142781100152848384"),
				DateTimeOffset.Now,
				"this is a cool sentence"
			);
			SentenceRaw sentenceRaw = new(sentence);
			sentenceRaw.OID.Should().Be(sentence.OID.ToString());
			sentenceRaw.Author.Should().Be(sentence.Author.ToString());
			sentenceRaw.Date.Should().Be(sentence.Date.ToString("o"));
			sentenceRaw.Text.Should().Be(sentence.Text);
		}

		[Fact]
		public void SentenceRawToSentenceTest()
		{
			DateTimeOffset now = DateTimeOffset.Now;
			SentenceRaw sentenceRaw = new(
				"Discord:discord.com:1253189664655806606:1253189664655806610:1253270827257036801:1254631007395643422:1254631136596852797:1",
				"Discord:discord.com:142781100152848384",
				now.ToString("o"),
				"this is a cool sentence"
			);
			Sentence sentence = sentenceRaw.ToSentence();
			sentence.OID.Should().BeEquivalentTo(ObjectOIDParser.Parse(sentenceRaw.OID));
			sentence.Author.Should().BeEquivalentTo(AuthorOID.Parse(sentenceRaw.Author));
			sentence.Date.Should().Be(now);
			sentence.Text.Should().Be(sentenceRaw.Text);
		}
	}
}
