using System.Text.RegularExpressions;

namespace AtelierTomato.Markov.Model.ObjectOID
{
	public class InvalidObjectOID : IObjectOID
	{
		public ServiceType Service { get; } = ServiceType.Invalid;
		public string Instance { get; } = "_";
		public string Type { get; set; }
		public InvalidObjectOID(string type)
		{
			if (type is "PermissionDenied")
			{
				Type = type;
			}
			else
			{
				throw new ArgumentException($"{type} is not a valid InvalidObjectOID Type.", nameof(type));
			}
		}
		public static InvalidObjectOID Parse(string OID)
		{
			if (string.IsNullOrWhiteSpace(OID))
				throw new ArgumentException("The OID given is empty.", nameof(OID));

			Regex invalidOIDRegex = OIDPattern.Generate([nameof(ServiceType), nameof(Instance), nameof(Type)]);

			var match = invalidOIDRegex.Match(OID);

			if (!match.Success)
				throw new ArgumentException("The OID given is not a valid InvalidObjectOID.", nameof(OID));

			if (match.Groups[nameof(ServiceType)].Value != ServiceType.Invalid.ToString())
				throw new ArgumentException("The OID given is not an InvalidObjectOID, as it does not begin with Invalid.", nameof(OID));

			if (match.Groups[nameof(Instance)].Value != "_")
				throw new ArgumentException("The InvalidObjectOID given is invalid, as it has an Instance other than _", nameof(OID));

			if (!match.Groups[nameof(Type)].Success)
				throw new ArgumentException("The InvalidObjectOID given is invalid, as it does not have a Type.", nameof(OID));

			return new InvalidObjectOID(match.Groups[nameof(Type)].Value);
		}
		public override string ToString()
		{
			var oidBuilder = new OIDBuilder(Service);
			oidBuilder.Append(Instance).Append(Type);
			return oidBuilder.Build();
		}
	}
}
