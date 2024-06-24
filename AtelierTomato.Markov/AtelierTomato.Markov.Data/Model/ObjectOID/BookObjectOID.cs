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
		public static BookObjectOID ForInstance(string instance)
			=> new(instance);
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
		public static BookObjectOID Parse(string OID)
		{
			string[] stringRange = ObjectOIDEscapement.Split(OID).ToArray();
			if (stringRange.Length > 7)
			{
				throw new ArgumentException("The OID given has too many members to be a valid BookObjectOID.");
			}
			if (stringRange[0] == string.Empty)
			{
				throw new ArgumentException("The OID given is empty.");
			}
			if (ServiceType.Book.ToString() != stringRange.First())
			{
				throw new ArgumentException("The OID given is not a BookObjectOID, as it does not begin with Book.");
			}

			if (stringRange.Length == 1)
			{
				throw new ArgumentException("The OID given is not long enough. You cannot have an IObjectOID with only the ServiceType.");
			} else if (stringRange.Length == 2)
			{
				return ForInstance(stringRange[1]);
			} else if (stringRange.Length == 3)
			{
				return ForSeries(stringRange[1], stringRange[2]);
			} else if (stringRange.Length == 4)
			{
				return ForBook(stringRange[1], stringRange[2], stringRange[3]);
			} else if (stringRange.Length == 5)
			{
				return ForChapter(stringRange[1], stringRange[2], stringRange[3], stringRange[4]);
			} else if (stringRange.Length >= 6)
			{
				if (!int.TryParse(stringRange[5], out int paragraph))
				{
					throw new ArgumentException("The part of the BookObjectOID corresponding to the paragraph was not able to be parsed into an integer value.");
				} else
				{
					if (stringRange.Length != 7)
					{
						return ForParagraph(stringRange[1], stringRange[2], stringRange[3], stringRange[4], paragraph);
					} else
					{
						if (!int.TryParse(stringRange[6], out int sentence))
						{
							throw new ArgumentException("The part of the BookObjectOID corresponding to the sentence was not able to be parsed into an integer value.");
						} else
						{
							return ForSentence(stringRange[1], stringRange[2], stringRange[3], stringRange[4], paragraph, sentence);
						}
					}
				}
			}
			throw new ArgumentException("Somehow, BookObjectOID.Parse() went through all of its code without returning a value. This should not happen.");
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
