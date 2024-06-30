using AtelierTomato.Markov.Data.Model.ObjectOID;
using FluentAssertions;

namespace AtelierTomato.Markov.Data.Test
{
	public class BookObjectOIDTest
	{
		[Fact]
		public void BookInstanceToStringTest()
		{
			BookObjectOID book = BookObjectOID.ForInstance("_");
			book.ToString().Should().Be("Book:_");
		}
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

		[Fact]
		public void BookInstanceParseTest()
		{
			BookObjectOID book = BookObjectOID.Parse("Book:_");
			book.Should().BeEquivalentTo(BookObjectOID.ForInstance("_"));
		}
		[Fact]
		public void BookSeriesParseTest()
		{
			BookObjectOID book = BookObjectOID.Parse("Book:_:Alice in Wonderland");
			book.Should().BeEquivalentTo(BookObjectOID.ForSeries("_", "Alice in Wonderland"));
		}
		[Fact]
		public void BookBookParseTest()
		{
			BookObjectOID book = BookObjectOID.Parse("Book:_:Alice in Wonderland:Through the Looking Glass");
			book.Should().BeEquivalentTo(BookObjectOID.ForBook("_", "Alice in Wonderland", "Through the Looking Glass"));
		}
		[Fact]
		public void BookChapterParseTest()
		{
			BookObjectOID book = BookObjectOID.Parse("Book:_:Alice in Wonderland:Through the Looking Glass:1");
			book.Should().BeEquivalentTo(BookObjectOID.ForChapter("_", "Alice in Wonderland", "Through the Looking Glass", "1"));
		}
		[Fact]
		public void BookParagraphParseTest()
		{
			BookObjectOID book = BookObjectOID.Parse("Book:_:Alice in Wonderland:Through the Looking Glass:1:3");
			book.Should().BeEquivalentTo(BookObjectOID.ForParagraph("_", "Alice in Wonderland", "Through the Looking Glass", "1", 3));
		}
		[Fact]
		public void BookSentenceParseTest()
		{
			BookObjectOID book = BookObjectOID.Parse("Book:_:Alice in Wonderland:Through the Looking Glass:1:3:2");
			book.Should().BeEquivalentTo(BookObjectOID.ForSentence("_", "Alice in Wonderland", "Through the Looking Glass", "1", 3, 2));
		}
		[Fact]
		public void BookParseUnescapeTest()
		{
			BookObjectOID book = BookObjectOID.Parse("Book:_:Alice^: in Wonderland:Through^^the^^Looking^^Glass");
			book.Should().BeEquivalentTo(BookObjectOID.ForBook("_", "Alice: in Wonderland", "Through^the^Looking^Glass"));
		}

		[Fact]
		public void BookParseTooLongTest()
		{
			var exception = Assert.Throws<ArgumentException>(() => BookObjectOID.Parse("Book:_:Alice in Wonderland:Through the Looking Glass:1:3:2:4"));
			Assert.Equal("The OID given is not a valid DiscordObjectOID.", exception.Message);
		}
		[Fact]
		public void BookParseEmptyTest()
		{
			var exception = Assert.Throws<ArgumentException>(() => BookObjectOID.Parse(string.Empty));
			Assert.Equal("The OID given is empty.", exception.Message);
		}
		[Fact]
		public void BookParseNotABookTest()
		{
			var exception = Assert.Throws<ArgumentException>(() => BookObjectOID.Parse("Invalid:1:Appleseed:???:4:Greg"));
			Assert.Equal("The OID given is not a BookObjectOID, as it does not begin with Book.", exception.Message);
		}
		[Fact]
		public void BookParseOnlyHasSerivceTypeTest()
		{
			var exception = Assert.Throws<ArgumentException>(() => BookObjectOID.Parse("Book"));
			Assert.Equal("The OID given is not a valid DiscordObjectOID.", exception.Message);
		}
		[Fact]
		public void BookParseParagraphNotIntTest()
		{
			var exception = Assert.Throws<ArgumentException>(() => BookObjectOID.Parse("Book:_:Alice in Wonderland:Through the Looking Glass:1:Lol"));
			Assert.Equal("The part of the BookObjectOID corresponding to the paragraph was not able to be parsed into an integer value.", exception.Message);
		}
		[Fact]
		public void BookParseSentenceNotIntTest()
		{
			var exception = Assert.Throws<ArgumentException>(() => BookObjectOID.Parse("Book:_:Alice in Wonderland:Through the Looking Glass:1:3:Wrong"));
			Assert.Equal("The part of the BookObjectOID corresponding to the sentence was not able to be parsed into an integer value.", exception.Message);
		}
	}
}
