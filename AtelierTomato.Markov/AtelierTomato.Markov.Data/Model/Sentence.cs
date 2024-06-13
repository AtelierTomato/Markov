namespace AtelierTomato.Markov.Data.Model
{
	public record Sentence(
		Guid ID, ObjectOID OID, AuthorOID Author, DateTimeOffset Date, string Text
	);
}
