namespace AtelierTomato.Markov.Data
{
	// All string version of Sentence, used as a return type of queries that .ToString() the Sentence in however they store the Sentence.
	public record SentenceRaw
	(
		string OID, string Author, string Date, string Text
	);
}
