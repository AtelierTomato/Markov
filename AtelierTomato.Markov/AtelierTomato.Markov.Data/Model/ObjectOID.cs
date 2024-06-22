﻿namespace AtelierTomato.Markov.Data.Model
{
	public record ObjectOID
	(
		ServiceType Service, string Instance, string Guild, string Channel, string Subchannel, string Message, int Fragment
	);
}
