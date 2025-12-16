using AtelierTomato.Markov.Model;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Core.Cooldown
{
	public class Cooldown
	{
		private CooldownOptions options;
		public Cooldown(IOptions<CooldownOptions> options)
		{
			this.options = options.Value;
		}

		private Dictionary<(AuthorOID author, IObjectOID location, CooldownType cooldownType), DateTimeOffset> cooldownTimeDictionary = [];
		public void WriteCooldownTime(AuthorOID author, IObjectOID location, CooldownType cooldownType)
		{
			var key = (author, location, cooldownType);
			cooldownTimeDictionary[key] = DateTimeOffset.Now;
		}

		/// <summary>
		/// Checks if this action is affected by a cooldown.
		/// </summary>
		/// <param name="author"></param>
		/// <param name="location"></param>
		/// <param name="cooldownType"></param>
		/// <returns>True if the action is permitted, false is cooldown is active.</returns>
		public bool CheckCooldown(AuthorOID author, IObjectOID location, CooldownType cooldownType)
		{
			var key = (author, location, cooldownType);
			if (cooldownTimeDictionary.TryGetValue(key, out var cooldownTime))
			{
				var commandCooldown = cooldownType switch
				{
					CooldownType.Speak => options.SpeakCooldown,
					CooldownType.PermissionsCheck => options.PermissionsCheckCooldown,
					CooldownType.MessagesCheck => options.MessagesCheckCooldown,
					CooldownType.ImageDownload => options.ImageDownloadCooldown,
					CooldownType.ImageUpload => options.ImageUploadCooldown,
					_ => options.GeneralCooldown
				};
				return DateTimeOffset.Now - cooldownTime > commandCooldown;
			}
			else
			{
				return true;
			}
		}

		public bool HandleCooldown(AuthorOID author, IObjectOID location, CooldownType cooldownType)
		{
			if (!CheckCooldown(author, location, cooldownType))
			{
				return false;
			}
			else
			{
				WriteCooldownTime(author, location, cooldownType);
				return true;
			}
		}
	}
}
