using System.Text.RegularExpressions;

namespace AtelierTomato.Markov.Core
{
	public class EmojiUtils
	{
		private static readonly Regex CustomEmojiRegex = new Regex(@"^[a-zA-Z0-9_]+$");

		/// <summary>
		/// Determines whether the given emoji is a custom emoji name.
		/// </summary>
		/// <param name="emoji">The emoji string to check.</param>
		/// <returns>True if the emoji is a custom emoji name, false if it's a Unicode emoji or otherwise invalid.</returns>
		public static bool IsCustomEmoji(string emoji) => CustomEmojiRegex.IsMatch(emoji);
	}
}
