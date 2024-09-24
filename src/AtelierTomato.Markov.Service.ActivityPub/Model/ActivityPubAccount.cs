namespace AtelierTomato.Markov.Service.ActivityPub.Model
{
	public class ActivityPubAccount
	{
		public string Id { get; set; }
		public string Username { get; set; }
		public string Acct { get; set; }
		public string DisplayName { get; set; }
		public bool Locked { get; set; }
		public bool Discoverable { get; set; }
		public bool Bot { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
		public string Note { get; set; }
		public string Url { get; set; }
		public string Avatar { get; set; }
		public string AvatarStatic { get; set; }
		public string Header { get; set; }
		public int FollowersCount { get; set; }
		public int FollowingCount { get; set; }
		public int StatusesCount { get; set; }
		public DateTimeOffset? LastStatusAt { get; set; }
		public ActivityPubRole Role { get; set; }
		// Ignored fields: Emojis, Fields
		public ActivityPubAccount(string id, string username, string acct, string display_name, bool locked, bool discoverable, bool bot, DateTimeOffset created_at, string note, string url, string avatar, string avatar_static, string header, int followers_count, int following_count, int statuses_count, DateTimeOffset? last_status_at, ActivityPubRole role)
		{
			Id = id;
			Username = username;
			Acct = acct;
			DisplayName = display_name;
			Locked = locked;
			Discoverable = discoverable;
			Bot = bot;
			CreatedAt = created_at;
			Note = note;
			Url = url;
			Avatar = avatar;
			AvatarStatic = avatar_static;
			Header = header;
			FollowersCount = followers_count;
			FollowingCount = following_count;
			StatusesCount = statuses_count;
			LastStatusAt = last_status_at;
			Role = role;
		}
	}
}
