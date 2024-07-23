namespace AtelierTomato.Markov.Data.Model.ObjectOID.Parser
{
	public class DiscordObjectOIDParser : IParser<DiscordObjectOID>
	{
		public bool CanParse(string input) => OIDEscapement.Split(input).First() == ServiceType.Discord.ToString();

		public DiscordObjectOID Parse(string input) => DiscordObjectOID.Parse(input);
	}
}
