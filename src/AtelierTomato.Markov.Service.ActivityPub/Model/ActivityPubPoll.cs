namespace AtelierTomato.Markov.Service.ActivityPub.Model
{
	public class ActivityPubPoll
	{
		public string Id { get; set; }
		public DateTimeOffset ExpiresAt { get; set; }
		public bool Expired { get; set; }
		public bool Multiple { get; set; }
		public int VotesCount { get; set; }
		public bool Voted { get; set; }
		public IReadOnlyList<ActivityPubPollOption> Options { get; set; }
		public IReadOnlyList<ActivityPubEmoji> Emojis { get; set; }
		// Ignored fields: VotersCount, OwnVotes
		public ActivityPubPoll(string id, DateTimeOffset expiresAt, bool expired, bool multiple, int votesCount, bool voted, IReadOnlyList<ActivityPubPollOption> options, IReadOnlyList<ActivityPubEmoji> emojis)
		{
			Id = id;
			ExpiresAt = expiresAt;
			Expired = expired;
			Multiple = multiple;
			VotesCount = votesCount;
			Voted = voted;
			Options = options;
			Emojis = emojis;
		}
	}
}
