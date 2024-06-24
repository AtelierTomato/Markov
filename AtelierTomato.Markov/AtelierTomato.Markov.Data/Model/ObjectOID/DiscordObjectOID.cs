namespace AtelierTomato.Markov.Data.Model.ObjectOID
{
	public class DiscordObjectOID : IObjectOID
	{
		public ServiceType Service { get; } = ServiceType.Discord;
		public string Instance { get; set; }
		public ulong? Server { get; set; }
		public ulong? Channel { get; set; }
		public ulong? Message { get; set; }
		public int? Fragment { get; set; }
		private DiscordObjectOID(string instance, ulong? server = null, ulong? channel = null, ulong? message = null, int? fragment = null)
		{
			Instance = instance;
			Server = server;
			Channel = channel;
			Message = message;
			Fragment = fragment;
		}
		public static DiscordObjectOID ForInstance(string instance)
			=> new(instance);
		public static DiscordObjectOID ForServer(string instance, ulong server)
			=> new(instance, server);
		public static DiscordObjectOID ForChannel(string instance, ulong server, ulong channel)
			=> new(instance, server, channel);
		public static DiscordObjectOID ForMessage(string instance, ulong server, ulong channel, ulong message)
			=> new(instance, server, channel, message);
		public static DiscordObjectOID ForFragment(string instance, ulong server, ulong channel, ulong message, int fragment)
			=> new(instance, server, channel, message, fragment);
		public static DiscordObjectOID Parse(string OID) => throw new NotImplementedException();
		public override string ToString() => throw new NotImplementedException();
	}
}
