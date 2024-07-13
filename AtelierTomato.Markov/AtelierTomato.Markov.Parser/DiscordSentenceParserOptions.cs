namespace AtelierTomato.Markov.Parser
{
	public class DiscordSentenceParserOptions
	{
		public List<string> ReplacePrefixFrom { get; set; } = ["t!remindme", "t!remind"];
		public List<string> ReplacePrefixTo { get; set; } = ["remind me", "remind me"];
		public List<string> RemovePrefixes { get; set; } = ["whs", "whr", "\u0026meme", "\u0026caption2", "\u0026caption", "\u0026motivation", "Septapus wormhole send", "Septapus wormhole reply", "t!8ball", "666^8ball", "777^8ball", ".meme", ".caption", ".recaption"];
		public List<string> IgnorePrefixes { get; set; } = ["t!", "t@", "m?", ")", "!", ".", "\u0026", "dlm!", "y!", "s?", "Septapus", "s.", "ch!", "$", "/", "%", "666^", "777^", "?", "e!", "ed!", "d!", "h!", "a!", "o!", "p!", "m!", "b!", "~>", "->"];
	}
}
