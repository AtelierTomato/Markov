using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Console.Modules.Parameters
{
	public record GenerateParameter(
		int SentencesToGenerate,
		SentenceFilter filter,
		string? keyword,
		string? firstWord
	);
}
