namespace AtelierTomato.Markov.Core.Generation
{
	public class KeywordOptions
	{
		public int MinimumAppearancesForKeyword { get; set; } = 5;
		public List<string> IgnoreKeyword { get; set; } = [];
	}
}
