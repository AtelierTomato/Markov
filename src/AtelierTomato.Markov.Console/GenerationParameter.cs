using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Console
{
	public record GenerationParameter(
		int SentencesToGenerate,
		SentenceFilter filter,
		string? keyword,
		string? firstWord
	);
}
