using System.Globalization;
using System.Text.RegularExpressions;
using AtelierTomato.Markov.Bot.Discord.Core.CommandModules.ParameterTypes;

namespace AtelierTomato.Markov.Model.ObjectOID
{
	public class DiscordObjectOID : IObjectOID
	{
		public override ServiceType Service { get; } = ServiceType.Discord;
		public override string? Instance { get; set; }
		public ulong? Server { get; set; }
		public ulong? Category { get; set; }
		public ulong? Channel { get; set; }
		public ulong? Thread { get; set; }
		public ulong? Message { get; set; }
		public int? Sentence { get; set; }
		private DiscordObjectOID(string? instance = null, ulong? server = null, ulong? category = null, ulong? channel = null, ulong? thread = null, ulong? message = null, int? sentence = null)
		{
			Instance = instance;
			Server = server;
			Category = category;
			Channel = channel;
			Thread = thread;
			Message = message;
			Sentence = sentence;
		}
		public static DiscordObjectOID ForService()
			=> new();
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
			if (string.IsNullOrWhiteSpace(OID))
			{
				throw new ArgumentException("The OID given is empty.", nameof(OID));
			}

			Regex discordOIDRegex = OIDPattern.Generate([nameof(ServiceType), nameof(Instance), nameof(Server), nameof(Category), nameof(Channel), nameof(Thread), nameof(Message), nameof(Sentence)]);

			var match = discordOIDRegex.Match(OID);

			if (!match.Success)
				throw new ArgumentException("The OID given is not a valid DiscordObjectOID.", nameof(OID));

			if (match.Groups[nameof(ServiceType)].Value != ServiceType.Discord.ToString())
				throw new ArgumentException("The OID given is not a DiscordObjectOID, as it does not begin with Discord.", nameof(OID));

			if (!match.Groups[nameof(Instance)].Success)
			{
				return ForService();
			}
			var instance = match.Groups[nameof(Instance)].Value;

			if (!match.Groups[nameof(Server)].Success)
			{
				return ForInstance(instance);
			}
			if (!ulong.TryParse(match.Groups[nameof(Server)].Value, out var serverId))
				throw new ArgumentException("The part of the DiscordObjectOID corresponding to the server was not able to be parsed into a ulong value.", nameof(OID));

			if (!match.Groups[nameof(Category)].Success)
			{
				return ForServer(instance, serverId);
			}
			if (!ulong.TryParse(match.Groups[nameof(Category)].Value, out var categoryId))
				throw new ArgumentException("The part of the DiscordObjectOID corresponding to the category was not able to be parsed into a ulong value.", nameof(OID));

			if (!match.Groups[nameof(Channel)].Success)
			{
				return ForCategory(instance, serverId, categoryId);
			}
			if (!ulong.TryParse(match.Groups[nameof(Channel)].Value, out var channelId))
				throw new ArgumentException("The part of the DiscordObjectOID corresponding to the channel was not able to be parsed into a ulong value.", nameof(OID));

			if (!match.Groups[nameof(Thread)].Success)
			{
				return ForChannel(instance, serverId, categoryId, channelId);
			}
			if (!ulong.TryParse(match.Groups[nameof(Thread)].Value, out var threadId))
				throw new ArgumentException("The part of the DiscordObjectOID corresponding to the thread was not able to be parsed into a ulong value.", nameof(OID));

			if (!match.Groups[nameof(Message)].Success)
			{
				return ForThread(instance, serverId, categoryId, channelId, threadId);
			}
			if (!ulong.TryParse(match.Groups[nameof(Message)].Value, out var messageId))
				throw new ArgumentException("The part of the DiscordObjectOID corresponding to the message was not able to be parsed into a ulong value.", nameof(OID));

			if (!match.Groups[nameof(Sentence)].Success)
			{
				return ForMessage(instance, serverId, categoryId, channelId, threadId, messageId);
			}
			if (!int.TryParse(match.Groups[nameof(Sentence)].Value, out var sentenceId))
				throw new ArgumentException("The part of the DiscordObjectOID corresponding to the sentence was not able to be parsed into an int value.", nameof(OID));

			return ForSentence(instance, serverId, categoryId, channelId, threadId, messageId, sentenceId);
		}

		public override IObjectOID Base()
		{
			if (Server is not null)
				return ForServer(Instance!, (ulong)Server);
			else
				return this;
		}

		public override string ToString()
		{
			var oidBuilder = new OIDBuilder(Service);
			if (Instance is not null)
			{
				oidBuilder.Append(Instance);
			}
			if (Server.HasValue)
			{
				oidBuilder.Append(Server.Value.ToString(CultureInfo.InvariantCulture));
			}
			else
			{
				return oidBuilder.Build();
			}
			if (Category.HasValue)
			{
				oidBuilder.Append(Category.Value.ToString(CultureInfo.InvariantCulture));
			}
			else
			{
				return oidBuilder.Build();
			}
			if (Channel.HasValue)
			{
				oidBuilder.Append(Channel.Value.ToString(CultureInfo.InvariantCulture));
			}
			else
			{
				return oidBuilder.Build();
			}
			if (Thread.HasValue)
			{
				oidBuilder.Append(Thread.Value.ToString(CultureInfo.InvariantCulture));
			}
			else
			{
				return oidBuilder.Build();
			}
			if (Message.HasValue)
			{
				oidBuilder.Append(Message.Value.ToString(CultureInfo.InvariantCulture));
			}
			else
			{
				return oidBuilder.Build();
			}
			if (Sentence.HasValue)
			{
				oidBuilder.Append(Sentence.Value.ToString(CultureInfo.InvariantCulture));
			}
			else
			{
				return oidBuilder.Build();
			}
			return oidBuilder.Build();
		}

		public DiscordObjectOID WithMessage(ulong message)
		{
			if (Channel is null)
			{
				throw new InvalidOperationException("A DiscordObjectOID cannot be returned with a Messagee if there is no value in Channel.");
			}
			return new DiscordObjectOID(Instance, Server, Category, Channel, Thread ?? 0, message);
		}

		public DiscordObjectOID WithSentence(int sentence)
		{
			if (Message is null)
			{
				throw new InvalidOperationException("A DiscordObjectOID cannot be returned with a Sentence if there is no value in Message.");
			}
			return new DiscordObjectOID(Instance, Server, Category, Channel, Thread, Message, sentence);
		}

		public DiscordObjectOID? ForLocationType(DiscordLocationType type)
		{
			return type switch
			{
				DiscordLocationType.Global => null,
				DiscordLocationType.Discord => ForService(),
				DiscordLocationType.Instance => Instance is not null ? ForInstance(Instance) : ThrowForLocationTypeError(type),
				DiscordLocationType.Server => Server is not null ? ForServer(Instance!, (ulong)Server) : ThrowForLocationTypeError(type),
				DiscordLocationType.Category => Category is not null ? ForCategory(Instance!, (ulong)Server!, (ulong)Category) : ThrowForLocationTypeError(type),
				DiscordLocationType.Channel => Channel is not null ? ForChannel(Instance!, (ulong)Server!, (ulong)Category!, (ulong)Channel) : ThrowForLocationTypeError(type),
				DiscordLocationType.Thread => Thread is not null ? ForThread(Instance!, (ulong)Server!, (ulong)Category!, (ulong)Channel!, (ulong)Thread) : ThrowForLocationTypeError(type),
				DiscordLocationType.Message => Message is not null ? ForMessage(Instance!, (ulong)Server!, (ulong)Category!, (ulong)Channel!, (ulong)Thread!, (ulong)Message) : ThrowForLocationTypeError(type),
				DiscordLocationType.Sentence => Sentence is not null ? ForSentence(Instance!, (ulong)Server!, (ulong)Category!, (ulong)Channel!, (ulong)Thread!, (ulong)Message!, (int)Sentence) : ThrowForLocationTypeError(type),
				_ => throw new ArgumentNullException(nameof(type))
			};
		}

		private static DiscordObjectOID ThrowForLocationTypeError(DiscordLocationType type)
		{
			throw new ArgumentOutOfRangeException(nameof(type), $"This object does not possess a '{type}' value.");
		}

		/// <summary>
		/// Returns the OID but with a different category.
		/// </summary>
		/// <param name="category"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public DiscordObjectOID UpdateCategory(ulong category)
		{
			if (Category is null)
				throw new InvalidOperationException($"Cannot update the category for {nameof(DiscordObjectOID)} '{this}' because it does not have a {nameof(Category)}.");

			if (!Channel.HasValue)
			{
				return ForCategory(Instance!, (ulong)Server!, category);
			}
			else if (!Thread.HasValue)
			{
				return ForChannel(Instance!, (ulong)Server!, category, (ulong)Channel);
			}
			else if (!Message.HasValue)
			{
				return ForThread(Instance!, (ulong)Server!, category, (ulong)Channel, (ulong)Thread);
			}
			else if (!Sentence.HasValue)
			{
				return ForMessage(Instance!, (ulong)Server!, category, (ulong)Channel, (ulong)Thread, (ulong)Message);
			}
			else
			{
				return ForSentence(Instance!, (ulong)Server!, category, (ulong)Channel, (ulong)Thread, (ulong)Message, (int)Sentence);
			}
		}
	}
}
