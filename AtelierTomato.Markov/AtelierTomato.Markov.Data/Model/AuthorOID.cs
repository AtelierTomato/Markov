namespace AtelierTomato.Markov.Data.Model
{
	public class AuthorOID
	{
		public ServiceType Service { get; set; }
		public string Instance { get; set; }
		public string Author { get; set; }
		public AuthorOID(ServiceType service, string instance, string author)
		{
			Service = service;
			Instance = instance;
			Author = author;
		}
		public string ToString() => string.Join(':', [Service.ToString(), OIDEscapement.Escape(Instance), OIDEscapement.Escape(Author)]);
	}
}
