namespace AtelierTomato.Markov.Data.Model.ObjectOID
{
	public class BookObjectOID : IObjectOID
	{
		public ServiceType Service { get; } = ServiceType.Book;
		public string Instance { get; set; }
		public string? Series { get; set; }
		public string? Book { get; set; }
		public string? Chapter { get; set; }
		public string? Paragraph { get; set; }
		public string? Sentence { get; set; }

		private BookObjectOID(string instance, string? series = null, string? book = null, string? chapter = null, string? paragraph = null, string? sentence = null)
		{
			Instance = instance;
			Series = series;
			Book = book;
			Chapter = chapter;
			Paragraph = paragraph;
			Sentence = sentence;
		}
		public static BookObjectOID ForSeries(string instance, string series)
			=> new(instance, series);
		public static BookObjectOID ForBook(string instance, string series, string book)
			=> new(instance, series, book);
		public static BookObjectOID ForChapter(string instance, string series, string book, string chapter)
			=> new(instance, series, book, chapter);
		public static BookObjectOID ForParagraph(string instance, string series, string book, string chapter, string paragraph)
			=> new(instance, series, book, chapter, paragraph);
		public static BookObjectOID ForSentence(string instance, string series, string book, string chapter, string paragraph, string sentence)
			=> new(instance, series, book, chapter, paragraph, sentence);
		public BookObjectOID Parse(string OID)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			throw new NotImplementedException();
		}
	}
}
