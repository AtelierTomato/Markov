using AtelierTomato.Markov.Core;
using AtelierTomato.Markov.Core.Generation;
using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Model.ObjectOID;
using AtelierTomato.Markov.Model.ObjectOID.LocationTypes;
using Discord;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Bot.Discord.Core
{
	public class HelpContentBuilder
	{
		private readonly DiscordBotOptions options;
		private readonly MarkovChainOptions markovChainOptions;
		private readonly SentenceParserOptions sentenceParserOptions;
		public HelpContentBuilder(IOptions<DiscordBotOptions> options, IOptions<MarkovChainOptions> markovChainOptions, IOptions<SentenceParserOptions> sentenceParserOptions)
		{
			this.options = options.Value;
			this.markovChainOptions = markovChainOptions.Value;
			this.sentenceParserOptions = sentenceParserOptions.Value;
		}

		public HelpSubject ParseSubject(string input)
		{
			switch (input)
			{
				case string when input.Equals("general", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("help", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("h", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.General;
				case string when input.Equals("scope", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.Scope;
				case string when input.Equals("location", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("iobjectoid", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("objectoid", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("discordobjectoid", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.Location;
				case string when input.Equals("author", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("authoroid", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.Author;
				case string when input.Equals("gettingstarted", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("gs", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.GettingStarted;
				case string when input.Equals("faq", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.FAQ;
				case string when input.Equals("optin", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("oi", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.OptIn;
				case string when input.Equals("optout", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("oo", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.OptOut;
				case string when input.Equals("permsuser", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("pu", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("permissionsuser", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.PermissionsUser;
				case string when input.Equals("deletepermission", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("dp", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.DeletePermission;
				case string when input.Equals("permsserver", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("ps", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("permserver", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("permissionsserver", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("permissionserver", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.PermissionsServer;
				case string when input.Equals("permsall", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("pa", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("permissionsall", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.PermissionsAll;
				case string when input.Equals("speak", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("s", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.Speak;
				case string when input.Equals("mimic", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("m", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.Mimic;
				case string when input.Equals("speakstartswith", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("ssw", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.SpeakStartsWith;
				case string when input.Equals("speakkeyword", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("sk", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.SpeakKeyword;
				case string when input.Equals("global", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("g", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.Global;
				case string when input.Equals("setlocationgroup", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("slg", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.SetLocationGroup;
				case string when input.Equals("locationgroup", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.LocationGroup;
				case string when input.Equals("createlocationgroup", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("clg", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.CreateLocationGroup;
				case string when input.Equals("renamelocationgroup", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("rlg", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.RenameLocationGroup;
				case string when input.Equals("deletelocationgroup", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("dlg", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.DeleteLocationGroup;
				case string when input.Equals("invitelocation", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("il", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.InviteLocation;
				case string when input.Equals("acceptlocationinvite", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("ali", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("acceptlocationgroupinvite", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("algi", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.AcceptLocationGroupInvite;
				case string when input.Equals("denylocationinvite", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("dli", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("denylocationgroupinvite", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("dlgi", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.DenyLocationGroupInvite;
				case string when input.Equals("updatelocation", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("ul", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.UpdateLocation;
				case string when input.Equals("removelocation", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("rl", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.RemoveLocation;
				case string when input.Equals("locationgrouprequestslist", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("lgrl", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("lrl", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("locationrequestslist", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("locationgrouprequestlist", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("locationrequestlist", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.LocationGroupRequestList;
				case string when input.Equals("locationgrouplist", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("lgl", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("locationgroupslist", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.LocationGroupList;
				case string when input.Equals("locationgroupinfo", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("lgi", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.LocationGroupInfo;
				case string when input.Equals("authorgroup", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.AuthorGroup;
				case string when input.Equals("createauthorgroup", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("cag", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.CreateAuthorGroup;
				case string when input.Equals("renameauthorgroup", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("rag", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.RenameAuthorGroup;
				case string when input.Equals("deleteauthorgroup", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("dag", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.DeleteAuthorGroup;
				case string when input.Equals("inviteauthor", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("ia", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.InviteAuthor;
				case string when input.Equals("acceptauthorinvite", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("aai", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("acceptauthorgroupinvite", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("aagi", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.AcceptAuthorGroupInvite;
				case string when input.Equals("denyauthorinvite", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("dai", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("denyauthorgroupinvite", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("dagi", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.DenyAuthorGroupInvite;
				case string when input.Equals("updateauthor", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("ua", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.UpdateAuthor;
				case string when input.Equals("removeauthor", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("ra", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.RemoveAuthor;
				case string when input.Equals("leaveauthorgroup", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("lag", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.LeaveAuthorGroup;
				case string when input.Equals("authorgrouprequestslist", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("agrl", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("arl", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("authorrequestslist", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("authorgrouprequestlist", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("authorrequestlist", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.AuthorGroupRequestList;
				case string when input.Equals("authorgrouplist", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("agl", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("authorgroupslist", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.AuthorGroupList;
				case string when input.Equals("authorgroupinfo", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("agi", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.AuthorGroupInfo;
				case string when input.Equals("retort", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("authorretortconfig", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("arc", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.Retort;
				case string when input.Equals("ping", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.Ping;
				case string when input.Equals("querysentences", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("qs", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.QuerySentences;
				case string when input.Equals("say", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.Say;
				case string when input.Equals("announce", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.Announce;
				case string when input.Equals("leave", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.Leave;
				case string when input.Equals("serverlist", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("servers", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("sl", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.ServerList;
				case string when input.Equals("channellist", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("channels", StringComparison.InvariantCultureIgnoreCase):
				case string when input.Equals("cl", StringComparison.InvariantCultureIgnoreCase):
					return HelpSubject.ChannelList;
				case string when input.Equals("", StringComparison.InvariantCultureIgnoreCase):
				default: return HelpSubject.Unknown;
			}
		}

		public EmbedBuilder BuildForSubject(HelpSubject subject)
		{
			var helpEmbed = subject switch
			{
				HelpSubject.Unknown => throw new ArgumentNullException(nameof(subject)),
				HelpSubject.General => new EmbedBuilder
				{
					Title = "Help",
					Description = $"For explanations of each command, say `{options.BotPrefix}help` and then the command name. " +
					$"Most commands have aliases that can be used instead of the command name that are the first letters of each word, such as `{options.BotPrefix}cag` or `{options.BotPrefix}oi`",
					Fields =
					{
						new EmbedFieldBuilder { IsInline = true, Name = "**Permission Commands**", Value = "`optin` `optout` `deletepermission` `permsuser` `permsserver` `permsall`" },
						new EmbedFieldBuilder { IsInline = true, Name = "**Speak Comamnds**", Value = "`speak` `mimic` `speakstartswith` `speakkeyword`" },
						new EmbedFieldBuilder { IsInline = true, Name = "**Settings Commands**", Value = "`global` `setlocationgroup` `retort`" },
						new EmbedFieldBuilder { IsInline = false, Name = "**LocationGroup Commands**", Value = "`createlocationgroup` `renamelocationgroup` `deletelocationgroup` `invitelocation` `acceptlocationinvite` `denylocationinvite` `updatelocation` `removelocation` `locationrequestlist` `locationgrouplist` `locationgroupinfo`" },
						new EmbedFieldBuilder { IsInline = false, Name = "**AuthorGroup Commands**", Value = "`createauthorgroup` `renameauthorgroup` `deleteauthorgroup` `inviteauthor` `acceptauthorinvite` `denyauthorinvite` `updateauthor` `removeauthor` `leaveauthorgroup` `authorrequestlist` `authorgrouplist` `authorgroupinfo`" },
						new EmbedFieldBuilder { IsInline = true, Name = "**Other Commands**", Value = "`ping` `querysentences` `gettingstarted` `faq`" },
						new EmbedFieldBuilder { IsInline = true, Name = "**Developer Commands**", Value = "`say` `announce` `leave` `serverlist` `channellist`" },
						new EmbedFieldBuilder { IsInline = true, Name = "**Other Help Topics**", Value = "`scope` `location` `author` `locationgroup` `authorgroup`"}
					}
				},
				HelpSubject.Scope => new EmbedBuilder
				{
					Title = "Scope Help",
					Description = $"Scopes are used in multiple commands as parameters for {options.BotName}. " +
					"The scopes along with a brief description them can be found below. " +
					"Note that some scopes cannot be used in certain commands.",
					Fields =
					{
						new EmbedFieldBuilder{ IsInline = false, Name = $"**{DiscordLocationType.Global}**", Value = "This scope refers to all places the bot has access to, this can be on or off-platform." },
						new EmbedFieldBuilder{ IsInline = false, Name = $"**{DiscordLocationType.Discord}**", Value = $"This scope refers to all of the current platform you are on, Discord. If Discord ever ends up having other {DiscordLocationType.Instance}s, this will refer to them as well." },
						new EmbedFieldBuilder{ IsInline = false, Name = $"**{DiscordLocationType.Instance}**", Value = $"This scope refers to the specific Instance of Discord that you are on. If Discord ever ends up having other {DiscordLocationType.Instance}s, this will only refer to the one you are on right now. This is essentially the same as the Discord Scope otherwise." },
						new EmbedFieldBuilder{ IsInline = false, Name = $"**{DiscordLocationType.Server}**", Value = "The current Discord Server you are in." },
						new EmbedFieldBuilder{ IsInline = false, Name = $"**{DiscordLocationType.Category}**", Value = "The current Discord Category that you are in." },
						new EmbedFieldBuilder{ IsInline = false, Name = $"**{DiscordLocationType.Channel}**", Value = "The current Discord Channel that you are in. This includes Voice or Text Channels, as well as Forum channels, but not Thread Channels." },
						new EmbedFieldBuilder{ IsInline = false, Name = $"**{DiscordLocationType.Thread}**", Value = "The current Discord Thread you are in. If you are not in a thread, this will refer to the lack of a Thread in a Channel. This is important, as otherwise using Channel will inherently imply Threads under the Channel." },
						new EmbedFieldBuilder{ IsInline = false, Name = $"**{DiscordLocationType.Message}**", Value = "The Discord Message that the bot is currently reading where you executed your command. This is usually not used." },
						new EmbedFieldBuilder{ IsInline = false, Name = $"**{DiscordLocationType.Sentence}**", Value = "The Sentence in the Discord Message that the bot is currently reading. Sentences are separated by standard sentence-ending punctuation, as well as newlines. This is usually not used." }
					}
				},
				HelpSubject.Location => new EmbedBuilder
				{
					Title = "Location Help",
					Description = $"A Location, also known as an {nameof(IObjectOID)}, is how {options.BotName} internally stores locations from differing platforms. " +
					$"A Location is made up of pieces that represent increasingly specific places in a hierarchy, for example, a {nameof(DiscordObjectOID)} has a Service (Discord), Instance (discord.com), Server (some server ID), Category (some category ID), et cetera. " +
					$"Some commands accept a Location instead of a Scope (see `{options.BotPrefix}help {HelpSubject.Scope}`) for a parameter. " +
					$"An {nameof(IObjectOID)} can cut off at any section, so some may go as far as Sentence or as near as Server or even Service. " +
					$"This is considered an advanced feature, and, generally, if you can use a Scope instead, you should, as writing a Location by hand is difficult. " +
					$"However, any commands that list out messages, permissions, et cetera, will give you a Location that you can copy and paste into a command." +
					Environment.NewLine + Environment.NewLine +
					$"Again, it is not recommended to freehand a Location, but if you must, here is a rough explanation of how a full {nameof(DiscordObjectOID)} is shaped:" +
					Environment.NewLine +
					$"With Placeholders: Discord:{options.DiscordInstance}:[Server ID]:[Category ID (0 if not in a category)]:[Channel ID]:[Thread ID (0 if not in a thread)]:[Message ID]:[Sentence (this is a number counting from 1 to the amount of sentences in a message)]" +
					Environment.NewLine +
					$"Full Example: Discord:{options.DiscordInstance}:1312182108013465620:1318052764559081514:1318052815092056094:0:1360138310235852951:1" +
					Environment.NewLine + Environment.NewLine +
					"Locations may refer to other platforms, and, in that case, they will not be formatted the same as the examples here.",
					Fields = { new EmbedFieldBuilder { IsInline = false, Name = "**TL;DR**", Value = $"You don't need to know what this is or how it works, just copy and paste whatever {nameof(IObjectOID)}s the bot spits out in list commands." } }
				},
				HelpSubject.Author => new EmbedBuilder
				{
					Title = "Author Help",
					Description = $"An Author, more formally known as an {nameof(AuthorOID)}, is how {options.BotName} internally stores users from differing platforms. " +
					$"An Author is made up of a Service, such as \"Discord\", an Instance, such as \"{options.DiscordInstance}\", and an ID that is representative of however the base platform records users, such as \"1050384196100706304\"." + Environment.NewLine +
					$"An example of a Discord {nameof(AuthorOID)} is `Discord:{options.DiscordInstance}:1050384196100706304`. " +
					$"In most cases where an {nameof(AuthorOID)} is accepted in {options.BotName}, a Discord User ID will also suffice, so you do not need to type out an {nameof(AuthorOID)} unless you are explicitly referring to an off-platform user.",
					Fields = { new EmbedFieldBuilder { IsInline = false, Name = "**TL;DR**", Value = $"You can usually just use a Discord user ID in place of an {nameof(AuthorOID)}" } },
				},
				HelpSubject.GettingStarted => new EmbedBuilder
				{
					Title = $"Getting Started with {options.BotName}",
					Description = $"{options.BotName} is a markov chain bot that uses *your* messages to generate funny sentences.",
					Fields =
					{
						new EmbedFieldBuilder { IsInline = false, Name = "**Opting In**", Value =
							$"For {options.BotName} to use your messages to generate sentences, you need to opt in. " +
							$"The recommended opt in command with parameters is `{options.BotPrefix}oi Global Global` if you do not care too much where your messages come from or end up. " +
							$"If you do care, you should read the information in `{options.BotPrefix}help oi`."
						},
						new EmbedFieldBuilder { IsInline = false, Name = "**Generating Sentences**", Value =
							$"To generate a markov sentence with {options.BotName}, you can simply say the bot's name. " +
							$"This will use your retort config, which can be further tweaked with `/retort` (see `{options.BotPrefix}help retort` for more info)."
						},
						new EmbedFieldBuilder { IsInline = false, Name = "**More Information**", Value =
							$"If you need to know more about {options.BotName}, you can use `{options.BotPrefix}help` to see all commands. " +
							$"You can also use `{options.BotPrefix}faq` to get the answers to some questions you may have."
						},
					}
				},
				HelpSubject.FAQ => new EmbedBuilder
				{
					Title = "Frequently Asked Questions",
					Description = "[fʌk ju]",
					Fields =
					{
						new EmbedFieldBuilder { IsInline = false, Name = $"**How can I make {options.BotName}'s outputs longer?**", Value = "Just keep talking." },
						new EmbedFieldBuilder { IsInline = false, Name = $"**Why do only messages {sentenceParserOptions.MinimumInputLength} words or longer count?**", Value = $"This is a minimum set in the settings by the developer to prevent shorter / more mundane sentences." },
						new EmbedFieldBuilder { IsInline = false, Name = "**If I'm not opted in, will my messages be added to the database?**", Value = "No." },
						new EmbedFieldBuilder { IsInline = false, Name = "**Will the bot add messages to the database from before I was opted in?**", Value = $"No, but if you want, you can add them yourself. React to the message in question with the {options.WriteEmojis.First()} emoji." },
						new EmbedFieldBuilder { IsInline = false, Name = "**Is there a way to remove my messages from the database?**", Value = $"Yes, though it is rudimentary. If you would like to remove a message from the database, react to it with the {options.DeleteEmojis.First()} emoji." },
						new EmbedFieldBuilder { IsInline = false, Name = $"**Can I look at the source code or host my own iteration of {options.BotName}?**", Value = $"You can, the source code can be found [here](https://github.com/AtelierTomato/Markov). However, if you just want the bot to only use your sentences, you can already configure that. See `{options.BotPrefix}help` for various settings and the like."},
						new EmbedFieldBuilder { IsInline = false, Name = $"**Why is {options.BotName} so stupid?**", Value = "Probably because you're teaching it." },
						new EmbedFieldBuilder { IsInline = false, Name = $"**Can {options.BotName} actually learn?**", Value = "Not really, but maybe it will be able to someday." },
						new EmbedFieldBuilder { IsInline = false, Name = $"**How does {options.BotName} actually work?**", Value =
							$"{options.BotName} gets the first word of a sentence in the database and adds it to a \"chain\" as well as an unfinished sentence. " +
							$"Then, {options.BotName} will match the chain's content to other sentences in the database, if it finds a match, it will add the next word after the last instance of the chain in the sentence to the chain and the unfinished sentence. " +
							$"If it does not find a match, it will prune the first word of the chain, and try again. " +
							$"It will also prune the chain if it gets longer than {markovChainOptions.MaximumPrevListLength}, as set by the bot developer. " +
							$"It will continue to do this until it fails to find anything and completely prunes the chain, or the sentence is {markovChainOptions.MaximumOutputLength} words long. " +
							$"Then, it will consider the sentence finished, render it, and send it. " +
							$"There's a bit more complexity to it than this, but that is the gist of it."
						},
					}
				},
				HelpSubject.OptIn => new EmbedBuilder
				{
					Title = "Opt In Help",
					Description = $"Opting in to message gathering allows {options.BotName} to gather your messages for use in generating markov chain sentences. " +
					$"This is done per a scope (such as `{DiscordLocationType.Server}` or `{DiscordLocationType.Channel}`, see `{options.BotPrefix}help {HelpSubject.Scope}` for more info) and to a scope. " +
					"The lowest Opt In setting will override higher settings, so if you opt into a server and out of a specific channel, only messages outside of that channel will be gathered, vice versa applies." +
					Environment.NewLine + Environment.NewLine +
					$"To opt in, use `{options.BotPrefix}optin` or `{options.BotPrefix}oo` and then two of any Scope in \"Valid Scopes\" (it can be the same Scope), the first being the \"from\" Scope (where the bot will gather messages from) and the second being the \"to\" Scope (where said messages will be available to use).",
					Fields =
					{
						new EmbedFieldBuilder{ IsInline = false, Name = "**Valid Scopes:**", Value = $"`{DiscordLocationType.Global}`, `{DiscordLocationType.Discord}`, `{DiscordLocationType.Instance}`, `{DiscordLocationType.Server}`, `{DiscordLocationType.Category}`, `{DiscordLocationType.Channel}`, `{DiscordLocationType.Thread}`" },
						new EmbedFieldBuilder{ IsInline = false, Name = "**Note:**", Value = "Two parameters are **REQUIRED**, you can no longer opt in with a single parameter." }
					},
					Footer = new EmbedFooterBuilder { Text = $"Example: {options.BotPrefix}optin {DiscordLocationType.Server} {DiscordLocationType.Global}" }
				},
				HelpSubject.OptOut => new EmbedBuilder
				{
					Title = "Opt Out Help",
					Description = $"Opting out of message gathering stops {options.BotName} from gathering your messages for a Scope (see {options.BotPrefix}help {HelpSubject.Scope}), as well as preventing any of your messages from the Scope in the database from being used by the markov generator. " +
					$"However, this does not delete the messages from the database. " +
					$"This is per-Scope or per-Location (see {options.BotPrefix}help {HelpSubject.Location}), and must be done per each Location you want to opt out of. " +
					$"You can use the command with a Location in order to opt out of a Location you no longer have access to." +
					Environment.NewLine + Environment.NewLine +
					$"To opt out, use `{options.BotPrefix}optout` or `{options.BotPrefix}oo` and then any of the below Scopes, or a Location ID.",
					Fields =
					{
						new EmbedFieldBuilder{ IsInline = false, Name = "**Valid Scopes:**", Value = $"`{DiscordLocationType.Global}`, `{DiscordLocationType.Discord}`, `{DiscordLocationType.Instance}`, `{DiscordLocationType.Server}`, `{DiscordLocationType.Category}`, `{DiscordLocationType.Channel}`, `{DiscordLocationType.Thread}`" },
						new EmbedFieldBuilder{ IsInline = false, Name = "**Note:**", Value = "The lowest (most specific) permission will override any higher permissions. " +
						$"This means if you opted in to a {DiscordLocationType.Server}, and then to a {DiscordLocationType.Channel}, and you opt out of the {DiscordLocationType.Server} later, then you will still be opted into the {DiscordLocationType.Channel}."}
					},
					Footer = new EmbedFooterBuilder { Text = $"Example: {options.BotPrefix}optout {DiscordLocationType.Server}" }
				},
				HelpSubject.DeletePermission => new EmbedBuilder
				{
					Title = "Delete Permission Help",
					Description = $"This command completely deletes a permission from {options.BotName} for a Location or a Scope (see `{options.BotPrefix}help {HelpSubject.Location}` and `{options.BotPrefix}help {HelpSubject.Location}`). " +
					$"This is different than opting out, as it acts as if the permission never existed at all. " +
					$"This command is mostly used in cases where you want to simplify the permissions that you have given to the bot, for example, if you opted in to both a Server and a Channel with the same permissions, if you delete the permission for the Channel, instead the bot will use the permissions for the Server for that channel. " +
					$"If you are instead want to explicitly deny permissions for a Location, instead, use `{options.BotPrefix}optout`. " +
					Environment.NewLine + Environment.NewLine +
					$"To delete a permission, use `{options.BotPrefix}deletepermission` or `{options.BotPrefix}dp` and then any of the below Scopes, or a Location ID.",
					Fields =
					{
						new EmbedFieldBuilder{ IsInline = false, Name = "**Valid Scopes:**", Value = $"`{DiscordLocationType.Global}`, `{DiscordLocationType.Discord}`, `{DiscordLocationType.Instance}`, `{DiscordLocationType.Server}`, `{DiscordLocationType.Category}`, `{DiscordLocationType.Channel}`, `{DiscordLocationType.Thread}`" },
					},
					Footer = new EmbedFooterBuilder { Text = $"Example: {options.BotPrefix}dp {DiscordLocationType.Channel}" }
				},
				HelpSubject.PermissionsUser => new EmbedBuilder
				{
					Title = "Permissions User Help",
					Description = "Sends you a message in DMs listing all the locations that you are opted into and to what locations.",
				},
				HelpSubject.PermissionsServer => new EmbedBuilder
				{
					Title = "Permissions Server Help",
					Description = "Only usable by server admins and bot developers. " +
					"Sends you a message in DMs that lists the permissions for all users that are opted into the server that you used the command in.",
				},
				HelpSubject.PermissionsAll => new EmbedBuilder
				{
					Title = "Permissions All Help",
					Description = "Only usable by bot developers. " +
					"Sends you a message in DMs that lists all of the permissions for all users and locations.",
				},
				HelpSubject.Speak => new EmbedBuilder
				{
					Title = "Speak Help",
					Description = "This command will generate a sentence using all of the sentences allowed for a Location. " +
					$"If you would like your sentences to be used in message generation, see `{options.BotPrefix}help {HelpSubject.OptIn}`."
				},
				HelpSubject.Mimic => new EmbedBuilder
				{
					Title = "Mimic Help",
					Description = "This command will generate a sentence using only your sentences that are allowed in a Location. " +
					$"If you are not opted in (see `{options.BotPrefix}help {HelpSubject.OptIn}` or do not have any sentences in the database, this command will fail to generate anything. " +
					$"If the bot has webhook permissions, mimic replies will appear using the user's name and avatar."
				},
				HelpSubject.SpeakStartsWith => new EmbedBuilder
				{
					Title = "Speak Starts With Help",
					Description = $"Similar to `{options.BotPrefix}speak`, but the parameter given will set the first word of the sentence."
				},
				HelpSubject.SpeakKeyword => new EmbedBuilder
				{
					Title = "Speak Keyword Help",
					Description = $"Similar to `{options.BotPrefix}speak`, but the parameter given will set the keyword of the sentence."
				},
				HelpSubject.Global => new EmbedBuilder
				{
					Title = "Global Help",
					Description = "Only usable by server admins and bot developers. " +
					$"Allows a server admin to enable {options.BotName} to use the global database when generating sentences in a Location. " +
					"By default, if given no arguments, it will enable or disable global settings for a server. " +
					$"Otherwise, it can take a Location or Scope (see `{options.BotPrefix}help {HelpSubject.Location}` and `{options.BotPrefix}help {HelpSubject.Scope}`) as a parameter to determine where to apply the setting. " +
					$"If a Location or a Scope is provided, and a second parameter \"Inherit\" is given, it will reset the global setting to inherit from higher up global settings, otherwise, it will switch global on or off explicitly.",
					Footer = new EmbedFooterBuilder { Text = $"Example: {options.BotPrefix}global Channel Inherit" }
				},
				HelpSubject.SetLocationGroup => new EmbedBuilder
				{
					Title = "Set LocationGroup Help",
					Description = $"Sets the LocationGroup (see `{options.BotPrefix}help {HelpSubject.LocationGroup}`) for a Location. " +
					$"Requires a Location or a Scope (see `{options.BotPrefix}help {HelpSubject.Location}` and `{options.BotPrefix}help {HelpSubject.Scope}`) as a parameter, and the ID of a LocationGroup. " +
					"To unset the LocationGroup for a Location, use only a Location and a Scope with this command, and no LocationGroup ID.",
					Footer = new EmbedFooterBuilder { Text = $"Example: {options.BotPrefix}slg Server 143" }
				},
				HelpSubject.LocationGroup => new EmbedBuilder
				{
					Title = "LocationGroup Help",
					Description = $"A LocationGroup is a group of Locations (see `{options.BotPrefix}help {HelpSubject.Location}`) with set permissions allowing for the pooling of usable messages across multiple locations. " +
					$"If you set a Location to use a certain LocationGroup using `{options.BotPrefix}setlocationgroup`, then all Locations in that group will contribute to the database that {options.BotName} pulls from when generating a sentence. " +
					$"Keep in mind that this does not change the availability of messages in the database, if a message is only allowed to be used in a specific server, being in a LocationGroup with that server will not change this. " +
					$"Instead, think of a LocationGroup as an alternative to Global being turned on, so your Location can use messages from any of the Location in the group. " +
					$"Every LocationGroup is assigned a numeric ID and must be given a name," +
					Environment.NewLine + Environment.NewLine +
					"Locations registered to a LocationGroup may have the following permissions assigned to them:",
					Fields =
					{
						new EmbedFieldBuilder { IsInline = false, Name = $"**{LocationGroupPermissionType.SentencesInGroup}**", Value = "Locations with this permission will contribute their Sentences to the group." },
						new EmbedFieldBuilder { IsInline = false, Name = $"**{LocationGroupPermissionType.UseGroup}**", Value = $"Locations with this permission are allowed to have their LocationGroup set to this LocationGroup, their owners may also use the LocationGroup in their retort configs." },
						new EmbedFieldBuilder { IsInline = false, Name = $"**{LocationGroupPermissionType.AddLocation}**", Value = "The owner of this Location may invite other Locations to the group, or update the permissions of other Locations in the group." },
						new EmbedFieldBuilder { IsInline = false, Name = $"**{LocationGroupPermissionType.RemoveLocation}**", Value = "The owner of this Location may remove other Locations from the group." },
						new EmbedFieldBuilder { IsInline = false, Name = $"**{LocationGroupPermissionType.RenameGroup}**", Value = "The owner of this Location may rename the LocationGroup." },
						new EmbedFieldBuilder { IsInline = false, Name = $"**{LocationGroupPermissionType.DeleteGroup}**", Value = "The owner of this Location may delete the LocationGroup." }
					}
				},
				HelpSubject.CreateLocationGroup => new EmbedBuilder
				{
					Title = "Create LocationGroup Help",
					Description = $"Creates a new LocationGroup (see `{options.BotPrefix}help {HelpSubject.LocationGroup}`) at provided Scope (see `{options.BotPrefix}help {HelpSubject.Scope}`) and with provided name.",
					Footer = new EmbedFooterBuilder { Text = $"Example: {options.BotPrefix}clg Server My New Group" }
				},
				HelpSubject.RenameLocationGroup => new EmbedBuilder
				{
					Title = "Rename LocationGroup Help",
					Description = $"Renames a LocationGroup (see `{options.BotPrefix}help {HelpSubject.LocationGroup}`) with given ID to given name.",
					Footer = new EmbedFooterBuilder { Text = $"Example: {options.BotPrefix}rlg 21 My Updated Name" }
				},
				HelpSubject.DeleteLocationGroup => new EmbedBuilder
				{
					Title = "Delete LocationGroup Help",
					Description = $"Deletes a LocationGroup (see `{options.BotPrefix}help {HelpSubject.LocationGroup}`) with given ID or given name.",
					Footer = new EmbedFooterBuilder { Text = $"Example: {options.BotPrefix}dlg My Old Group" }
				},
				HelpSubject.InviteLocation => new EmbedBuilder
				{
					Title = "Invite Location Help",
					Description = $"Invites a Location to a LocationGroup." + Environment.NewLine +
					Environment.NewLine + $"The first parameter must be the ID of the LocationGroup you'd like to invite the Location to." +
					Environment.NewLine + $"The second parameter must either be a Location or a Scope (see `{options.BotPrefix}help {HelpSubject.Location}` and `{options.BotPrefix}help {HelpSubject.Scope}`)." +
					Environment.NewLine + $"Any further parameters must be any combination of permissions (see `{options.BotPrefix}help {HelpSubject.LocationGroup}`) for the invited Location to have.",
					Footer = new EmbedFooterBuilder { Text = $"Example: {options.BotPrefix}il 23 Server SentencesInGroup UseGroup" }
				},
				HelpSubject.AcceptLocationGroupInvite => new EmbedBuilder
				{
					Title = "Accept LocationGroup Invite",
					Description = $"Accepts an invite to a LocationGroup (see `{options.BotPrefix}help {HelpSubject.LocationGroup}`)." + Environment.NewLine +
					Environment.NewLine + $"The first parameter is the Location or a Scope (see `{options.BotPrefix}help {HelpSubject.Location}` and `{options.BotPrefix}help {HelpSubject.Scope}`) to which the invitation was sent." +
					Environment.NewLine + $"The second parameter is the ID of the LocationGroup.",
					Footer = new EmbedFooterBuilder { Text = $"Example: {options.BotPrefix}ali Server 23" }
				},
				HelpSubject.DenyLocationGroupInvite => new EmbedBuilder
				{
					Title = "Deny LocationGroup Invite",
					Description = $"Denies an invite to a LocationGroup (see `{options.BotPrefix}help {HelpSubject.LocationGroup}`)." + Environment.NewLine +
					Environment.NewLine + $"The first parameter is the Location or a Scope (see `{options.BotPrefix}help {HelpSubject.Location}` and `{options.BotPrefix}help {HelpSubject.Scope}`) to which the invitation was sent." +
					Environment.NewLine + $"The second parameter is the ID of the LocationGroup.",
					Footer = new EmbedFooterBuilder { Text = $"Example: {options.BotPrefix}dli Server 23" }
				},
				HelpSubject.UpdateLocation => new EmbedBuilder
				{
					Title = "Update Location Help",
					Description = $"Updates the permissions of a Location in a LocationGroup." + Environment.NewLine +
					Environment.NewLine + $"The first parameter must be the ID of the LocationGroup you'd like to update the permissions of the Location in." +
					Environment.NewLine + $"The second parameter must either be a Location or a Scope (see `{options.BotPrefix}help {HelpSubject.Location}` and `{options.BotPrefix}help {HelpSubject.Scope}`)." +
					Environment.NewLine + $"Any further parameters must be any combination of permissions (see `{options.BotPrefix}help {HelpSubject.LocationGroup}`) for the Location to have.",
					Footer = new EmbedFooterBuilder { Text = $"Example: {options.BotPrefix}ul 23 Server SentencesInGroup UseGroup RenameGroup" }
				},
				HelpSubject.RemoveLocation => new EmbedBuilder
				{
					Title = "Remove Location Help",
					Description = $"Removes a Location from a LocationGroup (see `{options.BotPrefix}help {HelpSubject.LocationGroup}`), this can be used to remove a Location that you own even if you do not have the RemoveLocations permission." + Environment.NewLine +
					Environment.NewLine + $"The first parameter is the Location or a Scope (see `{options.BotPrefix}help {HelpSubject.Location}` and `{options.BotPrefix}help {HelpSubject.Scope}`) that you would like to remove from the group." +
					Environment.NewLine + $"The second parameter is the ID or name of the LocationGroup.",
					Footer = new EmbedFooterBuilder { Text = $"Example: {options.BotPrefix}rl Server 23" }
				},
				HelpSubject.LocationGroupRequestList => new EmbedBuilder
				{
					Title = "LocationGroup Request List Help",
					Description = $"Lists all invitations for Locations that you own to LocationGroups (see `{options.BotPrefix}help {HelpSubject.LocationGroup}`)."
				},
				HelpSubject.LocationGroupList => new EmbedBuilder
				{
					Title = "LocationGroup List Help",
					Description = $"Lists all LocationGroups (see `{options.BotPrefix}help {HelpSubject.LocationGroup}`) that Locations you own are registered to."
				},
				HelpSubject.LocationGroupInfo => new EmbedBuilder
				{
					Title = "LocationGroup Info Help",
					Description = $"Lists the name, permissions, and requests for a LocationGroup (see `{options.BotPrefix}help {HelpSubject.LocationGroup}`) with given ID."
				},
				HelpSubject.AuthorGroup => new EmbedBuilder
				{
					Title = "AuthorGroup Help",
					Description = $"An AuthorGroup is a group of Authors (see `{options.BotPrefix}help {HelpSubject.Author}`) with set permissions allowing for the pooling of usable messages across multiple authors. " +
					$"If you set your AuthorRetortConfig to use a certain AuthorGroup using `/retort` (see `{options.BotPrefix}help {HelpSubject.Retort}`), then all Authors in that group will contribute to the database that {options.BotName} pulls from when generating a sentence as a retort. " +
					$"Every AuthorGroup is assigned a numeric ID and must be given a name," +
					Environment.NewLine + Environment.NewLine +
					"Authors registered to an AuthorGroup may have the following permissions assigned to them:",
					Fields =
					{
						new EmbedFieldBuilder { IsInline = false, Name = $"**{AuthorGroupPermissionType.SentencesInGroup}**", Value = "Authors with this permission will contribute their Sentences to the group." },
						new EmbedFieldBuilder { IsInline = false, Name = $"**{AuthorGroupPermissionType.UseGroup}**", Value = "Authors with this permission are allowed to set their retort config to use this AuthorGroup." },
						new EmbedFieldBuilder { IsInline = false, Name = $"**{AuthorGroupPermissionType.AddAuthor}**", Value = "This Author may invite other Authors to the group, or update the permissions of other Authors in the group." },
						new EmbedFieldBuilder { IsInline = false, Name = $"**{AuthorGroupPermissionType.RemoveAuthor}**", Value = "This Author may remove other Authors from the group." },
						new EmbedFieldBuilder { IsInline = false, Name = $"**{AuthorGroupPermissionType.RenameGroup}**", Value = "This Author may rename the AuthorGroup." },
						new EmbedFieldBuilder { IsInline = false, Name = $"**{AuthorGroupPermissionType.DeleteGroup}**", Value = "This Author may delete the AuthorGroup." }
					}
				},
				HelpSubject.CreateAuthorGroup => new EmbedBuilder
				{
					Title = "Create AuthorGroup Help",
					Description = $"Creates a new AuthorGroup (see `{options.BotPrefix}help {HelpSubject.AuthorGroup}`) with provided name.",
					Footer = new EmbedFooterBuilder { Text = $"Example: {options.BotPrefix}cag My New Group" }
				},
				HelpSubject.RenameAuthorGroup => new EmbedBuilder
				{
					Title = "Rename AuthorGroup Help",
					Description = $"Renames an AuthorGroup (see `{options.BotPrefix}help {HelpSubject.AuthorGroup}`) with given ID to given name.",
					Footer = new EmbedFooterBuilder { Text = $"Example: {options.BotPrefix}rag 21 My Updated Name" }
				},
				HelpSubject.DeleteAuthorGroup => new EmbedBuilder
				{
					Title = "Delete AuthorGroup Help",
					Description = $"Deletes an AuthorGroup (see `{options.BotPrefix}help {HelpSubject.AuthorGroup}`) with given ID or given name.",
					Footer = new EmbedFooterBuilder { Text = $"Example: {options.BotPrefix}dag My Old Group" }
				},
				HelpSubject.InviteAuthor => new EmbedBuilder
				{
					Title = "Invite Author Help",
					Description = $"Invites an Author to an AuthorGroup." + Environment.NewLine +
					Environment.NewLine + $"The first parameter must be the ID of the AuthorGroup you'd like to invite the Author to." +
					Environment.NewLine + $"The second parameter must either be an Author (see `{options.BotPrefix}help {HelpSubject.Author}`) or a Discord User ID." +
					Environment.NewLine + $"Any further parameters must be any combination of permissions (see `{options.BotPrefix}help {HelpSubject.AuthorGroup}`) for the invited Author to have.",
					Footer = new EmbedFooterBuilder { Text = $"Example: {options.BotPrefix}ia 23 1050384196100706304 SentencesInGroup UseGroup" }
				},
				HelpSubject.AcceptAuthorGroupInvite => new EmbedBuilder
				{
					Title = "Accept AuthorGroup Invite",
					Description = $"Accepts an invite to the AuthorGroup (see `{options.BotPrefix}help {HelpSubject.AuthorGroup}`) with given name or ID.",
					Footer = new EmbedFooterBuilder { Text = $"Example: {options.BotPrefix}aai 23" }
				},
				HelpSubject.DenyAuthorGroupInvite => new EmbedBuilder
				{
					Title = "Deny AuthorGroup Invite",
					Description = $"Denies an invite to the AuthorGroup (see `{options.BotPrefix}help {HelpSubject.AuthorGroup}`) with given name or ID.",
					Footer = new EmbedFooterBuilder { Text = $"Example: {options.BotPrefix}dai 23" }
				},
				HelpSubject.UpdateAuthor => new EmbedBuilder
				{
					Title = "Update Author Help",
					Description = $"Updates the permissions of an Author in an AuthorGroup." + Environment.NewLine +
					Environment.NewLine + $"The first parameter must be the ID of the AuthorGroup you'd like to update the permissions of the Author in." +
					Environment.NewLine + $"The second parameter must either be an Author (see `{options.BotPrefix}help {HelpSubject.Author}`) or a Discord User ID." +
					Environment.NewLine + $"Any further parameters must be any combination of permissions (see `{options.BotPrefix}help {HelpSubject.AuthorGroup}`) for the Author to have.",
					Footer = new EmbedFooterBuilder { Text = $"Example: {options.BotPrefix}ul 23 1050384196100706304 SentencesInGroup UseGroup RenameGroup" }
				},
				HelpSubject.RemoveAuthor => new EmbedBuilder
				{
					Title = "Remove Author Help",
					Description = $"Removes an Author from an AuthorGroup (see `{options.BotPrefix}help {HelpSubject.AuthorGroup}`), this can only be used to remove other Authors, to remove yourself, use `{options.BotPrefix}lag`." + Environment.NewLine +
					Environment.NewLine + $"The first parameter is an Author (see `{options.BotPrefix}help {HelpSubject.Author}`) or a Discord User ID that you would like to remove from the group." +
					Environment.NewLine + $"The second parameter is the ID or name of the AuthorGroup.",
					Footer = new EmbedFooterBuilder { Text = $"Example: {options.BotPrefix}ra 1050384196100706304 23" }
				},
				HelpSubject.LeaveAuthorGroup => new EmbedBuilder
				{
					Title = "Leave AuthorGroup Help",
					Description = $"Removes you from an AuthorGroup (see `{options.BotPrefix}help {HelpSubject.AuthorGroup}`) with specified name or ID.",
					Footer = new EmbedFooterBuilder { Text = $"Example: {options.BotPrefix}lag My Out of Date Group" }
				},
				HelpSubject.AuthorGroupRequestList => new EmbedBuilder
				{
					Title = "AuthorGroup Request List Help",
					Description = $"Lists all of the invitations for AuthorGroups (see `{options.BotPrefix}help {HelpSubject.AuthorGroup}`) you have received."
				},
				HelpSubject.AuthorGroupList => new EmbedBuilder
				{
					Title = "AuthorGroup List Help",
					Description = $"Lists all AuthorGroups (see `{options.BotPrefix}help {HelpSubject.AuthorGroup}`) that you are registered to."
				},
				HelpSubject.AuthorGroupInfo => new EmbedBuilder
				{
					Title = "AuthorGroup Info Help",
					Description = $"Lists the name, permissions, and requests for an AuthorGroup (see `{options.BotPrefix}help {HelpSubject.AuthorGroup}`) with given ID."
				},
				HelpSubject.Retort => new EmbedBuilder
				{
					Title = "Retort Help",
					Description = $"The Retort command allows the user to set their {nameof(AuthorRetortConfig)}, which determines what settings {options.BotName} will use when generating retorts. " +
					$"Retorts are sentences generated either when a user says {options.BotName}'s name, or when a user replies to one of {options.BotName}'s posts. " +
					$"An {nameof(AuthorRetortConfig)} is tied to a specific Author and a specific Location (see `{options.BotPrefix}help {HelpSubject.Location}`), and the most specific {nameof(AuthorRetortConfig)} will be used, so if there is a config for both the Server and the Channel, the one for the Channel will be used. " +
					$"The retort command is only offered as a slash command, as it has a large amount of parameters that it can be given. " +
					$"Whenever the retort command is ran, it completely replaces the existing {nameof(AuthorRetortConfig)} for a location." +
					Environment.NewLine + Environment.NewLine +
					$"Below are explanations of the parameters that can be given when using `/retort`.",
					Fields =
					{
						new EmbedFieldBuilder { IsInline = false, Name = "**Location**", Value = $"This is the only required field, this can be either a Scope or a Location (see `{options.BotPrefix}help {HelpSubject.Scope}` and `{options.BotPrefix}help {HelpSubject.Location}`). If only a location is provided, the command will reset the retort config to empty." },
						new EmbedFieldBuilder { IsInline = false, Name = "**DisplayOption**", Value = "This can be either `Normal` or `Mimic`, Normal will have the bot just reply as a bot, Mimic will use a webhook to use your profile picture and name when retorting." },
						new EmbedFieldBuilder { IsInline = false, Name = "**AuthorFilter**", Value = $"This is a triple colon (:::) separated list of authors, this can be either a full AuthorOID (see `{options.BotPrefix}help {HelpSubject.Author}`) or a Discord User ID. This determines which Authors' messages will be used while generating a retort." },
						new EmbedFieldBuilder { IsInline = false, Name = "**LocationFilter**", Value = $"This is a triple colon (:::) separated list of either Scopes or Locations (see `{options.BotPrefix}help {HelpSubject.Scope}` and `{options.BotPrefix}help {HelpSubject.Location}`). This determines which Locations' messages will be used while generating a retort. Keep in mind the Location's settings still apply." },
						new EmbedFieldBuilder { IsInline = false, Name = "**AuthorGroup**", Value = $"This is the ID of the AuthorGroup that will be used for generation, an AuthorFilter will further filter the pool provided by the AuthorGroup." },
						new EmbedFieldBuilder { IsInline = false, Name = "**LocationGroup**", Value = $"This is the ID of the LocationGroup that will be used for generation, a LocationFilter will further filter the pool provided by the LocationGroup. Keep in mind the Location's settings still apply." },
						new EmbedFieldBuilder { IsInline = false, Name = "**Keyword**", Value = "This forces the keyword used when generating to be the given word, otherwise a keyword will be generated." },
						new EmbedFieldBuilder { IsInline = false, Name = "**FirstWord**", Value = "This forces the first word of the generated retort to be the given word." }
					},
					Footer = new EmbedFooterBuilder { Text = "Example: /retort location:server displayoption:mimic authorfilter:1289022865961648189" }
				},
				HelpSubject.Ping => new EmbedBuilder
				{
					Title = "Ping Help",
					Description = "Pings the bot! Pong!",
				},
				HelpSubject.QuerySentences => new EmbedBuilder
				{
					Title = "Query Sentences Help",
					Description = $"Allows you to query sentences from the database with a variety of filters. " +
					$"This command is only offered as a slash command due to having a variety of different parameters that can be applied to it. " +
					$"Queried sentences will be sent in DM regardless of where the command was sent from. " +
					$"Keep in mind that you can only filter by Authors that are in AuthorGroups that you have access to, and Locations that you own or that are in LocationGroups that you have access to." +
					Environment.NewLine + Environment.NewLine +
					$"Below are explanations of the parameters that can be given when using `/querysentences`.",
					Fields =
					{
						new EmbedFieldBuilder { IsInline = false, Name = "**AuthorGroup**", Value = "This is the ID of the AuthorGroup that will be used during querying, an AuthorFilter will further filter the pool provided by the AuthorGroup." },
						new EmbedFieldBuilder { IsInline = false, Name = "**LocationGroup**", Value = "This is the ID of the LocationGroup that will be used during querying, a LocationFilter will further filter the pool provided by the LocationGroup." },
						new EmbedFieldBuilder { IsInline = false, Name = "**AuthorFilter**", Value = $"This is a triple colon (:::) separated list of authors, this can be either a full AuthorOID (see `{options.BotPrefix}help {HelpSubject.Author}`) or a Discord User ID. This determines which Authors' sentences will be returned by the query." },
						new EmbedFieldBuilder { IsInline = false, Name = "**LocationFilter**", Value = $"This is a triple colon (:::) separated list of either Scopes or Locations (see `{options.BotPrefix}help {HelpSubject.Scope}` and `{options.BotPrefix}help {HelpSubject.Location}`). This determines which Locations' messages will be returned by the query." },
						new EmbedFieldBuilder { IsInline = false, Name = "**SearchString**", Value = "This is any string that you would like to search for, it can be a word or a group of words." },
						new EmbedFieldBuilder { IsInline = false, Name = "**Count**", Value = "This is the amount of sentences to return, by default this is set to 100." }
					}
				},
				HelpSubject.Say => new EmbedBuilder
				{
					Title = "Say Help",
					Description = "Allows the bot developer to make the bot parrot whatever is after the command name.",
					Footer = new EmbedFooterBuilder { Text = $"Example: {options.BotPrefix}say i'm dumb" }
				},
				HelpSubject.Announce => new EmbedBuilder
				{
					Title = "Announce Help",
					Description = "Allows the bot developer to send an announcement to the first available channel in every server that the bot is in.",
					Footer = new EmbedFooterBuilder { Text = $"Example: {options.BotPrefix}announce i just sold {options.BotName} to microsoft for $4 million" }
				},
				HelpSubject.Leave => new EmbedBuilder
				{
					Title = "Leave Help",
					Description = "Allows the bot developer to make the bot leave the server. " +
					"If no parameter is given, it will leave the server that it is currently in, otherwise, it takes the Discord ID of the server.",
					Footer = new EmbedFooterBuilder { Text = $"Example: {options.BotPrefix}leave 907121695779856394" }
				},
				HelpSubject.ServerList => new EmbedBuilder
				{
					Title = "Server List Help",
					Description = "Returns a list of servers that the bot is in."
				},
				HelpSubject.ChannelList => new EmbedBuilder
				{
					Title = "Channel List Help",
					Description = "Returns a list of channels in the current guild that the bot is in."
				},
				_ => throw new NotImplementedException()
			};
			helpEmbed.Color = options.EmbedColor;
			return helpEmbed;
		}
	}
}
