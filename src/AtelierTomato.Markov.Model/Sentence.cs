namespace AtelierTomato.Markov.Model
{
	public record Sentence
	(
		IObjectOID OID, AuthorOID Author, DateTimeOffset Date, string Text
	);
}
