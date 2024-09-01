namespace AtelierTomato.Markov.Model
{
	public record UserPermission
	(
		AuthorOID Author, IObjectOID OriginScope, IObjectOID AllowedScope
	);
}
