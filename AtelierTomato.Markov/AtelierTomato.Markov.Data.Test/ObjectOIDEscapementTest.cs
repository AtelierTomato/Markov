using AtelierTomato.Markov.Data.Model;
using FluentAssertions;

namespace AtelierTomato.Markov.Data.Test
{
	public class ObjectOIDEscapementTest
	{
		[Theory]
		[InlineData("Re:Birth", "Re^:Birth")]
		[InlineData("4^2", "4^^2")]
		[InlineData("Re:Birth4^2", "Re^:Birth4^^2")]
		[InlineData("4^2^3", "4^^2^^3")]
		[InlineData("apple:orange:grape", "apple^:orange^:grape")]
		[InlineData("4^^2", "4^^^^2")]
		[InlineData("b::c", "b^:^:c")]
		[InlineData("evenmore^:nonsense", "evenmore^^^:nonsense")]
		public void EscapeTest(string input, string output)
		{
			var result = ObjectOIDEscapement.Escape(input);

			result.Should().Be(output);
		}

		[Theory]
		[InlineData("Re^:Birth", "Re:Birth")]
		[InlineData("4^^2", "4^2")]
		[InlineData("Re^:Birth4^^2", "Re:Birth4^2")]
		[InlineData("4^^2^^3", "4^2^3")]
		[InlineData("apple^:orange^:grape", "apple:orange:grape")]
		[InlineData("4^^^^2", "4^^2")]
		[InlineData("b^:^:c", "b::c")]
		[InlineData("evenmore^^^:nonsense", "evenmore^:nonsense")]
		public void UnescapeTest(string input, string output)
		{
			var result = ObjectOIDEscapement.Unescape(input);

			result.Should().Be(output);
		}

		[Theory]
		[InlineData("apple:orange:grape", new[] { "apple", "orange", "grape" })]
		[InlineData("bread", new[] { "bread" })]
		[InlineData("Re^:Birth", new[] { "Re:Birth" })]
		[InlineData("Flop^^4", new[] { "Flop^4" })]
		[InlineData("apple^::bread", new[] { "apple:", "bread" })]
		[InlineData("apple::bread", new[] { "apple", "", "bread" })]
		public void SplitTest(string input, IEnumerable<string> output)
		{
			var result = ObjectOIDEscapement.Split(input);

			result.Should().BeEquivalentTo(output);
		}
	}
}