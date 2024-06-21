namespace AtelierTomato.Markov.Data.Model
{
	public record ObjectOID
	(
		ServiceType Service, string instance, string guild, string channel, string subchannel, string user, string message, string fragment
	);
}
