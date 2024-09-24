namespace AtelierTomato.Markov.Service.ActivityPub.Model
{
	public class ActivityPubEmoji
	{
		public string Shortcode { get; set; }
		public string Url { get; set; }
		public string StaticUrl { get; set; }
		public bool VisibleInPicker { get; set; }
		public ActivityPubEmoji(string shortcode, string url, string static_url, bool visible_in_picker)
		{
			Shortcode = shortcode;
			Url = url;
			StaticUrl = static_url;
			VisibleInPicker = visible_in_picker;
		}
	}
}
