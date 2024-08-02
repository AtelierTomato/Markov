namespace AtelierTomato.Markov.Model.ObjectOID.Parser
{
	public class BookObjectOIDParser : IParser<BookObjectOID>
	{
		public bool CanParse(string input) => OIDEscapement.Split(input).First() == ServiceType.Book.ToString();

		public BookObjectOID Parse(string input) => BookObjectOID.Parse(input);
	}
}
