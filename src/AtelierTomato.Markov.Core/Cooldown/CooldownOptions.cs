namespace AtelierTomato.Markov.Core.Cooldown
{
	public class CooldownOptions
	{
		public TimeSpan SpeakCooldown { get; set; } = new(0, 0, 3);
		public TimeSpan PermissionsCheckCooldown { get; set; } = new(0, 1, 0);
		public TimeSpan MessagesCheckCooldown { get; set; } = new(0, 2, 0);
		public TimeSpan GeneralCooldown { get; set; } = new(0, 0, 2);
		public TimeSpan ImageDownloadCooldown { get; set; } = new(3, 0, 0);
		public TimeSpan ImageUploadCooldown { get; set; } = new(3, 0, 0);
		public TimeSpan WebhookFailCooldown { get; set; } = new(1, 0, 0);
	}
}
