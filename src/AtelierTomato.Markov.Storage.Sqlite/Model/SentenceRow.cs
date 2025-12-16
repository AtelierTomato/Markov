using System.Globalization;
using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage.Sqlite.Model
{
	// All string version of Sentence, used as a return type of queries that .ToString() the Sentence in however they store the Sentence.
	internal sealed class SentenceRow
	{
		public string OID { get; set; } = string.Empty;
		public string Author { get; set; } = string.Empty;
		public string Date { get; set; } = string.Empty;
		public string Text { get; set; } = string.Empty;
		public SentenceRow() { }
		public SentenceRow(string OID, string author, string date, string text)
		{
			this.OID = OID;
			Author = author;
			Date = date;
			Text = text;
		}
		public SentenceRow(Sentence sentence)
		{
			OID = sentence.OID.ToString();
			Author = sentence.Author.ToString();
			Date = sentence.Date.ToString("o");
			Text = sentence.Text;
		}
		public Sentence ToSentence(MultiParser<IObjectOID> objectOIDParser)
		{
			DateTimeOffset.TryParseExact(Date, "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date);
			return new Sentence(objectOIDParser.Parse(OID), AuthorOID.Parse(Author), date, Text);
		}
	}
}
