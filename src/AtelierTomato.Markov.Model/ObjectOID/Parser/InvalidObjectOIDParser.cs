namespace AtelierTomato.Markov.Model.ObjectOID.Parser
{
	public class InvalidObjectOIDParser : IParser<InvalidObjectOID>
	{
		public bool CanParse(string input) => OIDEscapement.Split(input).First() == ServiceType.Invalid.ToString();

		public InvalidObjectOID Parse(string input) => InvalidObjectOID.Parse(input);
	}

}
