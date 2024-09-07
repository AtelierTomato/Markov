namespace AtelierTomato.Markov.Model
{
	public record AuthorPermission
	(
		AuthorOID Author, IObjectOID QueryScope, IObjectOID AllowedScope
	);
}
