namespace AtelierTomato.Markov.Service.ActivityPub.Model
{
	public class ActivityPubPost
	{
		public string Id { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
		public string? InReplyToId { get; set; }
		public string? InReplyToAccountId { get; set; }
		public bool Sensitive { get; set; }
		public string SpoilerText { get; set; }
		public string Visibility { get; set; }
		public string Language { get; set; }
		public string Uri { get; set; }
		public string Url { get; set; }
		public int RepliesCount { get; set; }
		public int ReblogsCount { get; set; }
		public int FavouritesCount { get; set; }
		public bool Favourited { get; set; }
		public bool Reblogged { get; set; }
		public bool Muted { get; set; }
		public bool Bookmarked { get; set; }
		public bool Pinned { get; set; }
		public string Content { get; set; }
		public ActivityPubPost? Reblog { get; set; }
		public ActivityPubAccount Account { get; set; }
		public IEnumerable<ActivityPubMention> Mentions { get; set; }
		public IEnumerable<ActivityPubTag> Tags { get; set; }
		public IEnumerable<ActivityPubEmoji> Emojis { get; set; }
		public ActivityPubPoll Poll { get; set; }
		public string Text { get; set; }
		// Ignored fields: MediaAttachments, Card
		public ActivityPubPost(string id, DateTimeOffset createdAt, string? inReplyToId, string? inReplyToAccountId, bool sensitive, string spoilerText, string visibility, string language, string uri, string url, int repliesCount, int reblogsCount, int favouritesCount, bool favourited, bool reblogged, bool muted, bool bookmarked, bool pinned, string content, ActivityPubPost? reblog, ActivityPubAccount account, IEnumerable<ActivityPubMention> mentions, IEnumerable<ActivityPubTag> tags, IEnumerable<ActivityPubEmoji> emojis, ActivityPubPoll poll, string text)
		{
			Id = id;
			CreatedAt = createdAt;
			InReplyToId = inReplyToId;
			InReplyToAccountId = inReplyToAccountId;
			Sensitive = sensitive;
			SpoilerText = spoilerText;
			Visibility = visibility;
			Language = language;
			Uri = uri;
			Url = url;
			RepliesCount = repliesCount;
			ReblogsCount = reblogsCount;
			FavouritesCount = favouritesCount;
			Favourited = favourited;
			Reblogged = reblogged;
			Muted = muted;
			Bookmarked = bookmarked;
			Pinned = pinned;
			Content = content;
			Reblog = reblog;
			Account = account;
			Mentions = mentions;
			Tags = tags;
			Emojis = emojis;
			Poll = poll;
			Text = text;
		}
	}
}
