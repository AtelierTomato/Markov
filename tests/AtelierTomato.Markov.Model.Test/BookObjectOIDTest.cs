﻿using AtelierTomato.Markov.Model.ObjectOID;
using FluentAssertions;

namespace AtelierTomato.Markov.Model.Test
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
			Action act = () => BookObjectOID.Parse("Book:_:Alice in Wonderland:Through the Looking Glass:1:3:2:4");
			act.Should().Throw<ArgumentException>().WithMessage("The OID given is not a valid DiscordObjectOID. (Parameter 'OID')");
		}
		[Fact]
		public void BookParseEmptyTest()
		{
			Action act = () => BookObjectOID.Parse(string.Empty);
			act.Should().Throw<ArgumentException>().WithMessage("The OID given is empty. (Parameter 'OID')");
		}
		[Fact]
		public void BookParseNotABookTest()
		{
			Action act = () => BookObjectOID.Parse("Invalid:1:Appleseed:???:4:Greg");
			act.Should().Throw<ArgumentException>().WithMessage("The OID given is not a BookObjectOID, as it does not begin with Book. (Parameter 'OID')");
		}
		[Fact]
		public void BookParseOnlyHasSerivceTypeTest()
		{
			Action act = () => BookObjectOID.Parse("Book");
			act.Should().Throw<ArgumentException>().WithMessage("The OID given is not a valid DiscordObjectOID. (Parameter 'OID')");
		}
		[Fact]
		public void BookParseParagraphNotIntTest()
		{
			Action act = () => BookObjectOID.Parse("Book:_:Alice in Wonderland:Through the Looking Glass:1:Lol");
			act.Should().Throw<ArgumentException>().WithMessage("The part of the BookObjectOID corresponding to the paragraph was not able to be parsed into an integer value. (Parameter 'OID')");
		}
		[Fact]
		public void BookParseSentenceNotIntTest()
		{
			Action act = () => BookObjectOID.Parse("Book:_:Alice in Wonderland:Through the Looking Glass:1:3:Wrong");
			act.Should().Throw<ArgumentException>().WithMessage("The part of the BookObjectOID corresponding to the sentence was not able to be parsed into an integer value. (Parameter 'OID')");
		}
		[Fact]
		public void IncrementBookOID()
		{
			BookObjectOID oid = BookObjectOID.ForParagraph("_", "_", "_", "_", 1);
			IEnumerable<string> strings = ["one", "two", "three"];

			List<Sentence> result = strings.Select((text, index) =>
				new Sentence(
					oid.WithSentence(index),
					new AuthorOID(ServiceType.Discord, "discord.com", "1"),
					DateTimeOffset.Now,
					text
				)
			).ToList();
			result[0].OID.Should().BeEquivalentTo(BookObjectOID.ForSentence("_", "_", "_", "_", 1, 0));
			result[1].OID.Should().BeEquivalentTo(BookObjectOID.ForSentence("_", "_", "_", "_", 1, 1));
			result[2].OID.Should().BeEquivalentTo(BookObjectOID.ForSentence("_", "_", "_", "_", 1, 2));
		}
	}
}
