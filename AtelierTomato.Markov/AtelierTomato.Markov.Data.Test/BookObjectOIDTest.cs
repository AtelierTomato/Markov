using AtelierTomato.Markov.Data.Model.ObjectOID;
using FluentAssertions;

namespace AtelierTomato.Markov.Data.Test
{
	public class BookObjectOIDTest
	{
		[Fact]
		public void BookSeriesToStringTest()
		{
			BookObjectOID book = BookObjectOID.ForSeries("_", "Alice in Wonderland");
			book.ToString().Should().Be("Book:_:Alice in Wonderland");
		}

		[Fact]
		public void BookBookToStringTest()
		{
			BookObjectOID book = BookObjectOID.ForBook("_", "Alice in Wonderland", "Through the Looking Glass");
			book.ToString().Should().Be("Book:_:Alice in Wonderland:Through the Looking Glass");
		}

		[Fact]
		public void BookChapterToStringTest()
		{
			BookObjectOID book = BookObjectOID.ForChapter("_", "Alice in Wonderland", "Through the Looking Glass", "1");
			book.ToString().Should().Be("Book:_:Alice in Wonderland:Through the Looking Glass:1");
		}

		[Fact]
		public void BookParagraphToStringTest()
		{
			BookObjectOID book = BookObjectOID.ForParagraph("_", "Alice in Wonderland", "Through the Looking Glass", "1", 3);
			book.ToString().Should().Be("Book:_:Alice in Wonderland:Through the Looking Glass:1:3");
		}

		[Fact]
		public void BookSentenceToStringTest()
		{
			BookObjectOID book = BookObjectOID.ForSentence("_", "Alice in Wonderland", "Through the Looking Glass", "1", 3, 2);
			book.ToString().Should().Be("Book:_:Alice in Wonderland:Through the Looking Glass:1:3:2");
		}

		[Fact]
		public void BookEscapeTest()
		{
			BookObjectOID book = BookObjectOID.ForSeries("_", "Book: The Beginning");
			book.ToString().Should().Be("Book:_:Book^: The Beginning");
		}
	}
}
