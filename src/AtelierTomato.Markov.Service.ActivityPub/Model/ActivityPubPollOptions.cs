namespace AtelierTomato.Markov.Service.ActivityPub.Model
{
	public class ActivityPubPollOptions
	{
		public string Title { get; set; }
		public int VotesCount { get; set; }
		public ActivityPubPollOptions(string title, int votesCount)
		{
			Title = title;
			VotesCount = votesCount;
		}
	}
}
