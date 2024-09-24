namespace AtelierTomato.Markov.Service.ActivityPub.Model
{
	public class ActivityPubPollOption
	{
		public string Title { get; set; }
		public int VotesCount { get; set; }
		public ActivityPubPollOption(string title, int votes_count)
		{
			Title = title;
			VotesCount = votes_count;
		}
	}
}
