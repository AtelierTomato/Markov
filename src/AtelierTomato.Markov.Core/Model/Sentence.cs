namespace AtelierTomato.Markov.Core.Model
{
	public record Sentence
	(
		IObjectOID OID, AuthorOID Author, DateTimeOffset Date, string Text
	);
}
