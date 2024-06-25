namespace AtelierTomato.Markov.Data.Model.ObjectOID.Parser
{
	public class BookObjectOIDParser : IParser<IObjectOID>
	{
		public bool CanParse(string input) => OIDEscapement.Split(input).First() == ServiceType.Book.ToString();

		public IObjectOID Parse(string input) => BookObjectOID.Parse(input);
	}
}
