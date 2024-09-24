namespace AtelierTomato.Markov.Service.ActivityPub.Model
{
	public class ActivityPubTag
	{
		public string Name { get; set; }
		public string Url { get; set; }
		public ActivityPubTag(string name, string url)
		{
			Name = name;
			Url = url;
		}
	}
}
