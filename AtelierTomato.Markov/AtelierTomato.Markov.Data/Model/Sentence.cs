namespace AtelierTomato.Markov.Data.Model
{
	public record Sentence
	(
		IObjectOID OID, AuthorOID Author, DateTimeOffset Date, string Text
	);
}
