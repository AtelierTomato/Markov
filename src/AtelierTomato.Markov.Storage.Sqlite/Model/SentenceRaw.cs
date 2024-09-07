using System.Globalization;
using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Model.ObjectOID.Parser;

namespace AtelierTomato.Markov.Storage.Sqlite.Model
{
	// All string version of Sentence, used as a return type of queries that .ToString() the Sentence in however they store the Sentence.
	internal sealed class SentenceRaw
	{
		public string OID { get; set; }
		public string Author { get; set; }
		public string Date { get; set; }
		public string Text { get; set; }
		private readonly MultiParser<IObjectOID> ObjectOIDParser = new([new SpecialObjectOIDParser(), new BookObjectOIDParser(), new DiscordObjectOIDParser()]);
		public SentenceRaw(string OID, string author, string date, string text)
		{
			this.OID = OID;
			Author = author;
			Date = date;
			Text = text;
		}
		public SentenceRaw(Sentence sentence)
		{
			OID = sentence.OID.ToString();
			Author = sentence.Author.ToString();
			Date = sentence.Date.ToString("o");
			Text = sentence.Text;
		}
		public Sentence ToSentence()
		{
			DateTimeOffset.TryParseExact(Date, "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date);
			return new Sentence(ObjectOIDParser.Parse(OID), AuthorOID.Parse(Author), date, Text);
		}
	}
}
