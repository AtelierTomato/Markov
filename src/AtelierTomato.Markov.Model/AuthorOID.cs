namespace AtelierTomato.Markov.Model
{
	public class AuthorOID(ServiceType service, string instance, string author) : IComparable<AuthorOID>
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
			}
			else
			{
				if (serviceType == ServiceType.Special)
				{
					throw new ArgumentException("The AuthorOID given is of ServiceType Special, which is not a valid ServiceType for AuthorOIDs.", nameof(OID));
				}
				else
				{
					return new AuthorOID(serviceType, stringRange[1], stringRange[2]);
				}
			}
		}

		public override bool Equals(object? obj)
		{
			if (obj is not AuthorOID other) return false;
			return Service == other.Service && Instance == other.Instance && Author == other.Author;
		}

		public override int GetHashCode() => HashCode.Combine(Service, Instance, Author);

		public int CompareTo(AuthorOID? other)
		{
			if (other is null) return 1;

			return string.Compare(ToString(), other.ToString(), StringComparison.Ordinal);
		}

		public static bool operator ==(AuthorOID? left, AuthorOID? right)
		{
			if (ReferenceEquals(left, right)) return true;
			if (left is null || right is null) return false;
			return left.Equals(right);
		}

		public static bool operator !=(AuthorOID? left, AuthorOID? right) => !(left == right);

		public static bool operator <(AuthorOID left, AuthorOID right)
		{
			return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
		}

		public static bool operator <=(AuthorOID left, AuthorOID right)
		{
			return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
		}

		public static bool operator >(AuthorOID left, AuthorOID right)
		{
			return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
		}

		public static bool operator >=(AuthorOID left, AuthorOID right)
		{
			return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
		}
	}
}
