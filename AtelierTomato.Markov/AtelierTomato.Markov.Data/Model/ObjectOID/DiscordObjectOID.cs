namespace AtelierTomato.Markov.Data.Model.ObjectOID
{
	public class DiscordObjectOID : IObjectOID
	{
		public ServiceType Service { get; } = ServiceType.Discord;
		public string Instance { get; set; }
		public ulong? Category { get; set; }
		public ulong? Server { get; set; }
		public ulong? Channel { get; set; }
		public ulong? Thread { get; set; }
		public ulong? Message { get; set; }
		public int? Sentence { get; set; }
		private DiscordObjectOID(string instance, ulong? server = null, ulong? category = null, ulong? channel = null, ulong? thread = null, ulong? message = null, int? sentence = null)
		{
			Instance = instance;
			Server = server;
			Category = category;
			Channel = channel;
			Thread = thread;
			Message = message;
			Sentence = sentence;
		}
		public static DiscordObjectOID ForInstance(string instance)
			=> new(instance);
		public static DiscordObjectOID ForServer(string instance, ulong server)
			=> new(instance, server);
		public static DiscordObjectOID ForCategory(string instance, ulong server, ulong category)
			=> new(instance, server, category);
		public static DiscordObjectOID ForChannel(string instance, ulong server, ulong category, ulong channel)
			=> new(instance, server, category, channel);
		public static DiscordObjectOID ForThread(string instance, ulong server, ulong category, ulong channel, ulong thread)
			=> new(instance, server, category, channel, thread);
		public static DiscordObjectOID ForMessage(string instance, ulong server, ulong category, ulong channel, ulong thread, ulong message)
			=> new(instance, server, category, channel, thread, message);
		public static DiscordObjectOID ForSentence(string instance, ulong server, ulong category, ulong channel, ulong thread, ulong message, int sentence)
			=> new(instance, server, category, channel, thread, message, sentence);
		public static DiscordObjectOID Parse(string OID)
		{
			string[] stringRange = OIDEscapement.Split(OID).ToArray();
			if (stringRange.Length > 8)
			{
				throw new ArgumentException("The OID given has too many members to be a valid DiscordObjectOID.");
			}
			if (stringRange[0] == string.Empty)
			{
				throw new ArgumentException("The OID given is empty.");
			}
			if (ServiceType.Discord.ToString() != stringRange.First())
			{
				throw new ArgumentException("The OID given is not a DiscordObjectOID, as it does not begin with Discord.");
			}
			if (stringRange.Length == 1)
			{
				throw new ArgumentException("The OID given is not long enough. You cannot have an IObjectOID with only the ServiceType.");
			} else if (stringRange.Length == 2)
			{
				return ForInstance(stringRange[1]);
			} else if (stringRange.Length >= 3)
			{
				if (!ulong.TryParse(stringRange[2], out ulong server))
				{
					throw new ArgumentException("The part of the DiscordObjectOID corresponding to the server was not able to be parsed into a ulong value.");
				} else if (stringRange.Length == 3)
				{
					return ForServer(stringRange[1], server);
				} else
				{
					if (!ulong.TryParse(stringRange[3], out ulong category))
					{
						throw new ArgumentException("The part of the DiscordObjectOID corresponding to the category was not able to be parsed into a ulong value.");
					} else if (stringRange.Length == 4)
					{
						return ForCategory(stringRange[1], server, category);
					} else
					{
						if (!ulong.TryParse(stringRange[4], out ulong channel))
						{
							throw new ArgumentException("The part of the DiscordObjectOID corresponding to the channel was not able to be parsed into a ulong value.");
						} else if (stringRange.Length == 5)
						{
							return ForChannel(stringRange[1], server, category, channel);
						} else
						{
							if (!ulong.TryParse(stringRange[5], out ulong thread))
							{
								throw new ArgumentException("The part of the DiscordObjectOID corresponding to the thread was not able to be parsed into a ulong value.");
							} else if (stringRange.Length == 6)
							{
								return ForThread(stringRange[1], server, category, channel, thread);
							} else
							{
								if (!ulong.TryParse(stringRange[6], out ulong message))
								{
									throw new ArgumentException("The part of the DiscordObjectOID corresponding to the message was not able to be parsed into a ulong value.");
								} else if (stringRange.Length == 7)
								{
									return ForMessage(stringRange[1], server, category, channel, thread, message);
								} else
								{
									if (!int.TryParse(stringRange[7], out int sentence))
									{
										throw new ArgumentException("The part of the DiscordObjectOID corresponding to the sentence was not able to be parsed into an int value.");
									} else
									{
										return ForSentence(stringRange[1], server, category, channel, thread, message, sentence);
									}
								}
							}
						}
					}
				}
			}
			throw new ArgumentException("Somehow, DiscordObjectOID.Parse() went through all of its code without returning a value. This should not happen.");
		}
		public override string ToString()
		{
			IEnumerable<string?> stringRange = [
				Service.ToString(),
				Instance,
				Server?.ToString() ?? null,
				Category?.ToString() ?? null,
				Channel?.ToString() ?? null,
				Thread?.ToString() ?? null,
				Message?.ToString() ?? null,
				Sentence?.ToString()?? null
			];
			stringRange = stringRange.OfType<string>().Select(OIDEscapement.Escape);
			return string.Join(':', stringRange);
		}
	}
}
