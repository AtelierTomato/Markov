namespace AtelierTomato.Markov.Model
{
	public record AuthorPermission
	(
		AuthorOID Author, IObjectOID OriginScope, IObjectOID AllowedScope
	);
}
