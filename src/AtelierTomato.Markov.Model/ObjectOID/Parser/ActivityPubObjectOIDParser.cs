namespace AtelierTomato.Markov.Model.ObjectOID.Parser
{
	public class ActivityPubObjectOIDParser : IParser<ActivityPubObjectOID>
	{
		public bool CanParse(string input) => OIDEscapement.Split(input).First() == ServiceType.ActivityPub.ToString();

		public ActivityPubObjectOID Parse(string input) => ActivityPubObjectOID.Parse(input);
	}

}
