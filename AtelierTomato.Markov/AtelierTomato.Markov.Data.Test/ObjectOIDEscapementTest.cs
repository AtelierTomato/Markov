using AtelierTomato.Markov.Data.Model.ObjectOID;
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
	}
}