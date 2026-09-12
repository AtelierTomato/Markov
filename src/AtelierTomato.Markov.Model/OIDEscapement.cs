using System.Text.RegularExpressions;

namespace AtelierTomato.Markov.Model
{
	public static class OIDEscapement
	{
		public static readonly Regex UnescapeRegex = new Regex(@"\^([\^:])");
		public static string Escape(string value) => new Regex(@"[\^:]").Replace(value, m => "^" + m.Value);
		public static string Unescape(string value) => UnescapeRegex.Replace(value, m => m.Groups[1].Value);
		public static IEnumerable<string> Split(string value) => Regex.Split(value, @"(?<!\^):").Select(Unescape);
	}
}
