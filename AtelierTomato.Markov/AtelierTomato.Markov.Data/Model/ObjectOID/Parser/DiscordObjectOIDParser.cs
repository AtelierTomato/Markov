namespace AtelierTomato.Markov.Data.Model.ObjectOID.Parser
{
	public class DiscordObjectOIDParser : IParser<IObjectOID>
	{
		public bool CanParse(string input) => OIDEscapement.Split(input).First() == ServiceType.Discord.ToString();

		public IObjectOID Parse(string input) => DiscordObjectOID.Parse(input);
	}
}
