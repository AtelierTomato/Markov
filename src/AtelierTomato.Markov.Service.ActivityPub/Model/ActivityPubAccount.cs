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
		public DateTimeOffset LastStatusAt { get; set; }
		public ActivityPubRole Role { get; set; }
		// Ignored fields: Emojis, Fields
		public ActivityPubAccount(string id, string username, string acct, string displayName, bool locked, bool discoverable, bool bot, DateTimeOffset createdAt, string note, string url, string avatar, string avatarStatic, string header, int followersCount, int followingCount, int statusesCount, DateTimeOffset lastStatusAt, ActivityPubRole role)
		{
			Id = id;
			Username = username;
			Acct = acct;
			DisplayName = displayName;
			Locked = locked;
			Discoverable = discoverable;
			Bot = bot;
			CreatedAt = createdAt;
			Note = note;
			Url = url;
			Avatar = avatar;
			AvatarStatic = avatarStatic;
			Header = header;
			FollowersCount = followersCount;
			FollowingCount = followingCount;
			StatusesCount = statusesCount;
			LastStatusAt = lastStatusAt;
			Role = role;
		}
	}
}
