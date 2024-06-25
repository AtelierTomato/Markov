namespace AtelierTomato.Markov.Data.Model.ObjectOID.Parser
{
	public class InvalidObjectOIDParser : IParser<IObjectOID>
	{
		public bool CanParse(string input) => OIDEscapement.Split(input).First() == ServiceType.Invalid.ToString();

		public IObjectOID Parse(string input) => throw new ArgumentException("The IObjectOID given is of ServiceType Invalid, which is not a valid ServiceType.");
	}

}
