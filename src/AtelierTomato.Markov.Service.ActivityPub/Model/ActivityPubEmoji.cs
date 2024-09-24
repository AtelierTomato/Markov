namespace AtelierTomato.Markov.Service.ActivityPub.Model
{
	public class ActivityPubEmoji
	{
		public string Shortcode { get; set; }
		public string Url { get; set; }
		public string StaticUrl { get; set; }
		public bool VisibleInPicker { get; set; }
		public ActivityPubEmoji(string shortcode, string url, string staticUrl, bool visibleInPicker)
		{
			Shortcode = shortcode;
			Url = url;
			StaticUrl = staticUrl;
			VisibleInPicker = visibleInPicker;
		}
	}
}
