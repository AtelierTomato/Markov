using System.Text.RegularExpressions;
using AtelierTomato.Markov.Model.ObjectOID.Types;

namespace AtelierTomato.Markov.Model.ObjectOID
{
	public class SpecialObjectOID : IObjectOID
	{
		public ServiceType Service { get; } = ServiceType.Special;
		public string Instance { get; } = "_";
		public SpecialObjectOIDType Type { get; set; }
		public SpecialObjectOID(string type)
		{
			if (!Enum.TryParse<SpecialObjectOIDType>(type, out var parsedType) || parsedType is SpecialObjectOIDType.Invalid)
				throw new ArgumentException($"{type} is not a valid SpecialObjectOID Type.", nameof(type));

			Type = parsedType;
		}
		public SpecialObjectOID(SpecialObjectOIDType type)
		{
			Type = type;
		}
		public static SpecialObjectOID Parse(string OID)
		{
			if (string.IsNullOrWhiteSpace(OID))
				throw new ArgumentException("The OID given is empty.", nameof(OID));

			Regex specialOIDRegex = OIDPattern.Generate([nameof(ServiceType), nameof(Instance), nameof(Type)]);

			var match = specialOIDRegex.Match(OID);

			if (!match.Success)
				throw new ArgumentException("The OID given is not a valid SpecialObjectOID.", nameof(OID));

			if (match.Groups[nameof(ServiceType)].Value != ServiceType.Special.ToString())
				throw new ArgumentException("The OID given is not an SpecialObjectOID, as it does not begin with Special.", nameof(OID));

			if (match.Groups[nameof(Instance)].Value != "_")
				throw new ArgumentException("The SpecialObjectOID given is invalid, as it has an Instance other than \"_\".", nameof(OID));

			if (!match.Groups[nameof(Type)].Success)
				throw new ArgumentException("The SpecialObjectOID given is invalid, as it does not have a Type.", nameof(OID));

			return new SpecialObjectOID(match.Groups[nameof(Type)].Value);
		}
		public override string ToString()
		{
			var oidBuilder = new OIDBuilder(Service);
			oidBuilder.Append(Instance).Append(Type.ToString());
			return oidBuilder.Build();
		}
	}
}
