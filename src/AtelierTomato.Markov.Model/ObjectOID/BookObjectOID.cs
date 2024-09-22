using System.Globalization;
using System.Text.RegularExpressions;

namespace AtelierTomato.Markov.Model.ObjectOID
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
			if (string.IsNullOrWhiteSpace(OID))
			{
				throw new ArgumentException("The OID given is empty.", nameof(OID));
			}

			Regex bookOIDRegex = OIDPattern.Generate([nameof(ServiceType), nameof(Instance), nameof(Series), nameof(Book), nameof(Chapter), nameof(Paragraph), nameof(Sentence)]);

			var match = bookOIDRegex.Match(OID);

			if (!match.Success)
				throw new ArgumentException("The OID given is not a valid BookObjectOID.", nameof(OID));

			if (match.Groups[nameof(ServiceType)].Value != ServiceType.Book.ToString())
				throw new ArgumentException("The OID given is not a BookObjectOID, as it does not begin with Book.", nameof(OID));

			var instance = OIDEscapement.Unescape(match.Groups[nameof(Instance)].Value);

			if (!match.Groups[nameof(Series)].Success)
			{
				return ForInstance(instance);
			}
			var series = OIDEscapement.Unescape(match.Groups[nameof(Series)].Value);

			if (!match.Groups[nameof(Book)].Success)
			{
				return ForSeries(instance, series);
			}
			var book = OIDEscapement.Unescape(match.Groups[nameof(Book)].Value);

			if (!match.Groups[nameof(Chapter)].Success)
			{
				return ForBook(instance, series, book);
			}
			var chapter = OIDEscapement.Unescape(match.Groups[nameof(Chapter)].Value);

			if (!match.Groups[nameof(Paragraph)].Success)
			{
				return ForChapter(instance, series, book, chapter);
			}
			if (!int.TryParse(match.Groups[nameof(Paragraph)].Value, out var paragraph))
				throw new ArgumentException("The part of the BookObjectOID corresponding to the paragraph was not able to be parsed into an integer value.", nameof(OID));

			if (!match.Groups[nameof(Sentence)].Success)
			{
				return ForParagraph(instance, series, book, chapter, paragraph);
			}
			if (!int.TryParse(match.Groups[nameof(Sentence)].Value, out var sentence))
				throw new ArgumentException("The part of the BookObjectOID corresponding to the sentence was not able to be parsed into an integer value.", nameof(OID));

			return ForSentence(instance, series, book, chapter, paragraph, sentence);
		}

		public override string ToString()
		{
			var oidBuilder = new OIDBuilder(Service);
			oidBuilder.Append(Instance);
			if (Series is not null)
			{
				oidBuilder.Append(Series);
			}
			else
			{
				return oidBuilder.Build();
			}
			if (Book is not null)
			{
				oidBuilder.Append(Book);
			}
			else
			{
				return oidBuilder.Build();
			}
			if (Chapter is not null)
			{
				oidBuilder.Append(Chapter);
			}
			else
			{
				return oidBuilder.Build();
			}
			if (Paragraph.HasValue)
			{
				oidBuilder.Append(Paragraph.Value.ToString(CultureInfo.InvariantCulture));
			}
			else
			{
				return oidBuilder.Build();
			}
			if (Sentence.HasValue)
			{
				oidBuilder.Append(Sentence.Value.ToString(CultureInfo.InvariantCulture));
			}
			else
			{
				return oidBuilder.Build();
			}
			return oidBuilder.Build();
		}
		public BookObjectOID WithSentence(int sentence)
		{
			if (Paragraph is null)
			{
				throw new InvalidOperationException("A BookObjectOID cannot increment Sentence if there is no value in Paragraph.");
			}
			return new BookObjectOID(Instance, Series, Book, Chapter, Paragraph, sentence);
		}
	}
}
