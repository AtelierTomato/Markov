namespace AtelierTomato.Markov.Data.Generation
{
	public class KeywordOptions
	{
		public int MinimumAppearancesForKeyword { get; set; } = 5;
		public List<string> IgnoreKeyword { get; set; } = [];
	}
}
