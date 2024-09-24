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
		public IReadOnlyList<ActivityPubMention> Mentions { get; set; }
		public IReadOnlyList<ActivityPubTag> Tags { get; set; }
		public IReadOnlyList<ActivityPubEmoji> Emojis { get; set; }
		public ActivityPubPoll? Poll { get; set; }
		public string Text { get; set; }
		// Ignored fields: MediaAttachments, Card
		public ActivityPubPost(string id, DateTimeOffset created_at, string? in_reply_to_id, string? in_reply_to_account_id, bool sensitive, string spoiler_text, string visibility, string language, string uri, string url, int replies_count, int reblogs_count, int favourites_count, bool favourited, bool reblogged, bool muted, bool bookmarked, bool pinned, string content, ActivityPubPost? reblog, ActivityPubAccount account, IReadOnlyList<ActivityPubMention> mentions, IReadOnlyList<ActivityPubTag> tags, IReadOnlyList<ActivityPubEmoji> emojis, ActivityPubPoll? poll, string text)
		{
			Id = id;
			CreatedAt = created_at;
			InReplyToId = in_reply_to_id;
			InReplyToAccountId = in_reply_to_account_id;
			Sensitive = sensitive;
			SpoilerText = spoiler_text;
			Visibility = visibility;
			Language = language;
			Uri = uri;
			Url = url;
			RepliesCount = replies_count;
			ReblogsCount = reblogs_count;
			FavouritesCount = favourites_count;
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
