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
		public IEnumerable<ActivityPubPollOptions> Options { get; set; }
		public IEnumerable<ActivityPubEmoji> Emojis { get; set; }
		// Ignored fields: VotersCount, OwnVotes
	}
}
