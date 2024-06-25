using AtelierTomato.Markov.Data.Model;

namespace AtelierTomato.Markov.Data
{
	public interface ISentenceAccess
	{
		/// <summary>
		/// Read a random <see cref="Sentence"/> that matches the given <paramref name="filter"/>.
		/// </summary>
		/// <param name="filter">The <see cref="SentenceFilter"/> that the resulting <see cref="Sentence"/> must match.</param>
		/// <returns></returns>
		Task<Sentence?> ReadSentence(SentenceFilter filter);
		/// <summary>
		/// Read a random sentence that matches to given <paramref name="filter"/> that is not in <paramref name="previousIDs"/> and matches to the <paramref name="prevList"/>.
		/// </summary>
		/// <param name="prevList">A list of previous words that the function will attempt to match to other <see cref="Sentence"/>s with.</param>
		/// <param name="previousIDs">A list of previous IDs of <see cref="Sentence"/>s that will be excluded from the query.</param>
		/// <param name="filter">The <see cref="SentenceFilter"/> that the resulting <see cref="Sentence"/> must match.</param>
		/// <returns></returns>
		Task<Sentence?> ReadNextSentence(List<string> prevList, List<string> previousIDs, SentenceFilter filter);
	}
}
