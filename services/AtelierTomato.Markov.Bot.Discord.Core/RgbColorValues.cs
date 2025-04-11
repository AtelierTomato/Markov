using Discord;

namespace AtelierTomato.Markov.Bot.Discord.Core
{
	public class RgbColorValues
	{
		public int Red { get; set; }
		public int Green { get; set; }
		public int Blue { get; set; }

		public static explicit operator Color(RgbColorValues color)
		{
			return new Color(color.Red, color.Green, color.Blue);
		}
	}
}
