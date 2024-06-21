using AtelierTomato.Markov.Data.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace AtelierTomato.Markov.Data
{
	public interface ISentenceAccess
	{
		Task<Sentence?> ReadSentence(IFilterHandler filter);
		Task<Sentence?> ReadNextSentence(List<string> prevList, List<Guid> previousIDs, IFilterHandler filter);
	}
}
