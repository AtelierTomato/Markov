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
		public static AuthorOID Parse(string OID)
		{
			string[] stringRange = OIDEscapement.Split(OID).ToArray();
			if (stringRange.Length > 3)
			{
				throw new ArgumentException("The OID given has too many members to be a valid AuthorOID");
			}
			if (stringRange[0] == string.Empty)
			{
				throw new ArgumentException("The OID given is empty");
			}
			if (!Enum.TryParse(stringRange.First(), out ServiceType serviceType))
			{
				throw new ArgumentException("The ServiceType was not able to be parsed from the given OID.");
			} else
			{
				if (serviceType == ServiceType.Invalid)
				{
					throw new ArgumentException("The AuthorOID given is of ServiceType Invalid, which is not a valid ServiceType.");
				} else
				{
					return new AuthorOID(serviceType, stringRange[1], stringRange[2]);
				}
			}
		}
	}
}
