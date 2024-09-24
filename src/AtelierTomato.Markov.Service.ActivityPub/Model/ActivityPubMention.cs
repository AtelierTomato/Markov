namespace AtelierTomato.Markov.Service.ActivityPub.Model
{
	public class ActivityPubMention
	{
		public string ID { get; set; }
		public string Username { get; set; }
		public string Url { get; set; }
		public string Acct { get; set; }
		public ActivityPubMention(string iD, string username, string url, string acct)
		{
			ID = iD;
			Username = username;
			Url = url;
			Acct = acct;
		}
	}
}
