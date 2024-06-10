namespace AtelierTomato.Markov.Data.Model
{
	public record Sentence(
		Guid ID, ObjectOID OID, AuthorOID Author, string Text
	);
}
