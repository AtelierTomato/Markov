namespace AtelierTomato.Markov.Data.Model
{
	public record Sentence
	(
		Guid ID, IObjectOID OID, AuthorOID Author, DateTimeOffset Date, string Text
	);
}
