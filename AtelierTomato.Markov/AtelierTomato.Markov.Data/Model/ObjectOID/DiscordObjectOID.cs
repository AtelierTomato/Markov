namespace AtelierTomato.Markov.Data.Model.ObjectOID
{
	public class DiscordObjectOID : IObjectOID
	{
		public ServiceType Service { get; } = ServiceType.Discord;
		public string Instance { get; set; }
		public ulong? Server { get; set; }
		public ulong? Channel { get; set; }
		public ulong? Message { get; set; }
		public int? Sentence { get; set; }
		private DiscordObjectOID(string instance, ulong? server = null, ulong? channel = null, ulong? message = null, int? sentence = null)
		{
			Instance = instance;
			Server = server;
			Channel = channel;
			Message = message;
			Sentence = sentence;
		}
		public static DiscordObjectOID ForInstance(string instance)
			=> new(instance);
		public static DiscordObjectOID ForServer(string instance, ulong server)
			=> new(instance, server);
		public static DiscordObjectOID ForChannel(string instance, ulong server, ulong channel)
			=> new(instance, server, channel);
		public static DiscordObjectOID ForMessage(string instance, ulong server, ulong channel, ulong message)
			=> new(instance, server, channel, message);
		public static DiscordObjectOID ForSentence(string instance, ulong server, ulong channel, ulong message, int sentence)
			=> new(instance, server, channel, message, sentence);
		public static DiscordObjectOID Parse(string OID) => throw new NotImplementedException();
		public override string ToString()
		{
			IEnumerable<string?> stringRange = [
				Service.ToString(),
				ObjectOIDEscapement.Escape(Instance),
				Server?.ToString() ?? null,
				Channel?.ToString() ?? null,
				Message?.ToString() ?? null,
				Sentence?.ToString()?? null
			];
			stringRange = stringRange.Where(s => s is not null);
			return string.Join(':', stringRange);
		}
	}
}
