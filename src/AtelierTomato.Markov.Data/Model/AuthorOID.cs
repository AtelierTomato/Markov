namespace AtelierTomato.Markov.Data.Model
{
	public class AuthorOID(ServiceType service, string instance, string author)
	{
		public ServiceType Service { get; set; } = service;
		public string Instance { get; set; } = instance;
		public string Author { get; set; } = author;

		public override string ToString() => string.Join(':', ((IEnumerable<string>)([Service.ToString(), Instance, Author])).Select(OIDEscapement.Escape));

		public static AuthorOID Parse(string OID)
		{
			string[] stringRange = OIDEscapement.Split(OID).ToArray();
			if (stringRange.Length > 3)
			{
				throw new ArgumentException("The OID given has too many members to be a valid AuthorOID.", nameof(OID));
			}
			if (string.IsNullOrWhiteSpace(stringRange[0]))
			{
				throw new ArgumentException("The OID given is empty", nameof(OID));
			}
			if (!Enum.TryParse(stringRange.First(), out ServiceType serviceType))
			{
				throw new ArgumentException("The ServiceType was not able to be parsed from the given OID.", nameof(OID));
			} else
			{
				if (serviceType == ServiceType.Invalid)
				{
					throw new ArgumentException("The AuthorOID given is of ServiceType Invalid, which is not a valid ServiceType.", nameof(OID));
				} else
				{
					return new AuthorOID(serviceType, stringRange[1], stringRange[2]);
				}
			}
		}
	}
}
