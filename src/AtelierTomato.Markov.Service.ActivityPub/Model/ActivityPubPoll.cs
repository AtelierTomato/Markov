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
		public ActivityPubPoll(string id, DateTimeOffset expires_at, bool expired, bool multiple, int votes_count, bool voted, IReadOnlyList<ActivityPubPollOption> options, IReadOnlyList<ActivityPubEmoji> emojis)
		{
			Id = id;
			ExpiresAt = expires_at;
			Expired = expired;
			Multiple = multiple;
			VotesCount = votes_count;
			Voted = voted;
			Options = options;
			Emojis = emojis;
		}
	}
}
