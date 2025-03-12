using Discord;

namespace AtelierTomato.Markov.Bot.Discord.Core
{
	public class DiscordBotOptions
	{
		public string BotName { get; set; } = "SET THIS";
		public string BotPrefix { get; set; } = "d!";
		public string DiscordInstance { get; set; } = "discord.com";
		public List<string> WriteEmojis { get; set; } = ["\uD83D\uDCDD"];
		public List<string> WriteDiscordEmojiNames { get; set; } = [];
		public List<string> DeleteEmojis { get; set; } = ["\u274C"];
		public List<string> DeleteDiscordEmojiNames { get; set; } = [];
		public string FailEmoji { get; set; } = "\uD83D\uDEAB";
		public string FailDiscordEmojiName { get; set; } = "";
		public string ActivityString { get; set; } = "Placeholder!";
		public ActivityType ActivityType { get; set; } = ActivityType.Playing;
		public string EmptyMarkovReturn { get; set; } = "The Markov chain failed to generate anything. This is likely because either the database or the specific query handed to it resulted in 0 Sentences.";
		public List<ulong> DeveloperIDs { get; set; } = [1050384196100706304, 142781100152848384];
	}
}
