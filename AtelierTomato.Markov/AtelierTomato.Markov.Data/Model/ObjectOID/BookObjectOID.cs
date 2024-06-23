namespace AtelierTomato.Markov.Data.Model.ObjectOID
{
	public class BookObjectOID : IObjectOID
	{
		public ServiceType Service { get; } = ServiceType.Book;
		public string Instance { get; set; }
		public string? Series { get; set; }
		public string? Book { get; set; }
		public string? Chapter { get; set; }
		public int? Paragraph { get; set; }
		public int? Sentence { get; set; }

		private BookObjectOID(string instance, string? series = null, string? book = null, string? chapter = null, int? paragraph = null, int? sentence = null)
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
		public static BookObjectOID ForParagraph(string instance, string series, string book, string chapter, int paragraph)
			=> new(instance, series, book, chapter, paragraph);
		public static BookObjectOID ForSentence(string instance, string series, string book, string chapter, int paragraph, int sentence)
			=> new(instance, series, book, chapter, paragraph, sentence);
		public BookObjectOID Parse(string OID)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			IEnumerable<string?> stringRange = [Service.ToString(), Instance, Series, Book, Chapter, Paragraph?.ToString() ?? null, Sentence?.ToString() ?? null];
			stringRange = stringRange.Where(s => s is not null);
			stringRange = stringRange.Select(ObjectOIDEscapement.Escape);
			return string.Join(':', stringRange);
		}
	}
}
