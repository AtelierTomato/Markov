using AtelierTomato.Markov.Data.Model.ObjectOID;

namespace AtelierTomato.Markov.Data.Model
{
	public interface IObjectOID
	{
		ServiceType Service { get; }
		string Instance { get; }
		string ToString();
	}
	public class IObjectOIDUtil
	{
		public static IObjectOID Parse(string OID)
		{
			if (!Enum.TryParse(OIDEscapement.Split(OID).First(), out ServiceType service))
			{
				throw new ArgumentException("The ServiceType was not able to be parsed from the given OID.");
			} else
			{
				return service switch {
					ServiceType.Invalid => throw new ArgumentException("The IObjectOID given is of ServiceType Invalid, which is not a valid ServiceType."),
					ServiceType.Book => BookObjectOID.Parse(OID),
					ServiceType.Discord => DiscordObjectOID.Parse(OID),
					_ => throw new ArgumentException("The IObjectOID given matched to a valid ServiceType, but there is no logic to Parse for that ServiceType. This should not happen."),
				};
			}
		}
	}
}
