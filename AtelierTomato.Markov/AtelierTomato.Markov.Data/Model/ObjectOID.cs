namespace AtelierTomato.Markov.Data.Model
{
	public record ObjectOID
	(
		Enum Service, string instance, string guild, string channel, string subchannel, string user, string message, string fragment
	);
}
