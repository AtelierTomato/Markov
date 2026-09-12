namespace AtelierTomato.Markov.Model.ObjectOID.Parser
{
	public class SpecialObjectOIDParser : IParser<SpecialObjectOID>
	{
		public bool CanParse(string input) => OIDEscapement.Split(input).First() == ServiceType.Special.ToString();

		public SpecialObjectOID Parse(string input) => SpecialObjectOID.Parse(input);
	}

}
