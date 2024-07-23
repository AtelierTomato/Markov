using AtelierTomato.Markov.Core.Model;
using AtelierTomato.Markov.Core.Model.ObjectOID.Parser;
using System.Globalization;

namespace AtelierTomato.Markov.Storage.Sqlite
{
	// All string version of Sentence, used as a return type of queries that .ToString() the Sentence in however they store the Sentence.
	internal class SentenceRaw
	{
		public string OID { get; set; }
		public string Author { get; set; }
		public string Date { get; set; }
		public string Text { get; set; }
		private readonly MultiParser<IObjectOID> ObjectOIDParser = new([new InvalidObjectOIDParser(), new BookObjectOIDParser(), new DiscordObjectOIDParser()]);
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
			DateTimeOffset.TryParseExact(Date, "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset date);
			return new Sentence(ObjectOIDParser.Parse(OID), AuthorOID.Parse(Author), date, Text);
		}
	}
}
