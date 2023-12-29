using AtelierTomato.Markov.Database.Model;

namespace AtelierTomato.Markov.Database
{
	public interface ISentenceAccess
	{
		Task<Sentence?> ReadSentence();
	}
}
