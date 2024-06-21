namespace AtelierTomato.Markov.Parser
{
	public interface ISentenceParser
	{
		public IEnumerable<string> TokenizeText(string text);
		public IEnumerable<string> ParseIntoSentenceTexts(string text);
		public string ProcessText(string text);

	}
}
