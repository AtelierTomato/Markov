using System.Text;
using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Model.ObjectOID;
using AtelierTomato.Markov.Service.Discord;
using AtelierTomato.Markov.Storage;
using ConsoleTableExt;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Bot.Discord.Core.CommandModules
{
	public class SentencesModule : InteractionModuleBase<SocketInteractionContext>
	{
		private readonly ISentenceAccess sentenceAccess;
		private readonly IAuthorPermissionAccess authoerPermissionAccess;
		private readonly ILocationAccess locationAccess;
		private readonly IAuthorGroupPermissionAccess authorGroupPermissionAccess;
		private readonly ILocationGroupPermissionAccess locationGroupPermissionAccess;
		private readonly DiscordObjectOIDBuilder objectOIDBuilder;
		private readonly MultiParser<IObjectOID> objectOIDParser;
		private readonly DiscordBotOptions options;
		public SentencesModule(ISentenceAccess sentenceAccess, IAuthorPermissionAccess authoerPermissionAccess, ILocationAccess locationAccess, IAuthorGroupPermissionAccess authorGroupPermissionAccess, ILocationGroupPermissionAccess locationGroupPermissionAccess, DiscordObjectOIDBuilder objectOIDBuilder, MultiParser<IObjectOID> objectOIDParser, IOptions<DiscordBotOptions> options)
		{
			this.sentenceAccess = sentenceAccess;
			this.authoerPermissionAccess = authoerPermissionAccess;
			this.locationAccess = locationAccess;
			this.authorGroupPermissionAccess = authorGroupPermissionAccess;
			this.locationGroupPermissionAccess = locationGroupPermissionAccess;
			this.objectOIDBuilder = objectOIDBuilder;
			this.objectOIDParser = objectOIDParser;
			this.options = options.Value;
		}

		[SlashCommand("querysentences", "Query sentences based on various parameters.")]
		public async Task QuerySentencesCommand(
			[Summary("authorgroup", "GUID for the author group")] string? authorGroup = null,
			[Summary("locationgroup", "GUID for the location group")] string? locationGroup = null,
			[Summary("authorfilter", "Double colon (::) separated list of authors")] string? authorFilter = null,
			[Summary("locationfilter", "Double colon (::) separated list of locations")] string? locationFilter = null,
			[Summary("searchstring", "Text to search for")] string? searchString = null,
			[Summary("count", "Number of sentences to return")] int count = 100
		)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var locationOID = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			var isDev = options.DeveloperIDs.Contains(Context.User.Id);    // Some guards only need to be checked if not a developer.
			if (!isDev && authorFilter is null && authorGroup is null && locationFilter is null && locationGroup is null)
			{
				await RespondAsync("only devs can query from all locations and all authors!", ephemeral: true);
				return;
			}
			List<AuthorOID> effectiveAuthorFilter = [];
			List<IObjectOID> effectiveLocationFilter = [];
			if (authorGroup is not null)
			{
				if (!Guid.TryParse(authorGroup, out var authorGroupID))
				{
					await RespondAsync("authorgroup was not a valid guid!", ephemeral: true);
					return;
				}

				if (!isDev)
				{
					var authorGroupPermission = await authorGroupPermissionAccess.ReadAuthorGroupPermission(authorGroupID, authorOID);
					if (authorGroupPermission is null || !authorGroupPermission.Permissions.HasFlag(AuthorGroupPermissionType.UseGroup))
					{
						await RespondAsync("you do not have access to this author group!", ephemeral: true);
						return;
					}
				}

				// Add authors that have sentences in the group
				var authorsFromGroup = (await authorGroupPermissionAccess.ReadAuthorGroupPermissionRangeByID(authorGroupID))
					.Where(agp => agp.Permissions.HasFlag(AuthorGroupPermissionType.SentencesInGroup))
					.Select(agp => agp.Author)
					.ToList();
				if (authorsFromGroup.Count is 0)
				{
					await RespondAsync("no authors were found in the author group...", ephemeral: true);
					return;
				}

				effectiveAuthorFilter = authorsFromGroup;
			}
			bool authorDoesNotHaveUsePermissionForLocationGroup = false;
			if (locationGroup is not null)
			{
				if (!Guid.TryParse(locationGroup, out var locationGroupID))
				{
					await RespondAsync("locationgroup was not a valid guid!", ephemeral: true);
					return;
				}

				if (!isDev)
				{
					var locationGroupPermission = await locationGroupPermissionAccess.ReadLocationGroupPermission(locationGroupID, locationOID);
					if (locationGroupPermission is null || !locationGroupPermission.Permissions.HasFlag(LocationGroupPermissionType.UseGroup))
					{
						await RespondAsync("this location does not have access to this location group!", ephemeral: true);
						return;
					}

					authorDoesNotHaveUsePermissionForLocationGroup = !(await locationGroupPermissionAccess.ReadLocationGroupPermissionsForOwner(locationGroupID, authorOID)).HasFlag(LocationGroupPermissionType.UseGroup);
					if (locationFilter is null && authorFilter is null && authorGroup is null && authorDoesNotHaveUsePermissionForLocationGroup)
					{
						await RespondAsync("since you do not have permissions in this location group, you cannot query all of its messages.", ephemeral: true);
						return;
					}
				}

				// Add locations that have sentences in the group
				var locationsFromGroup = (await locationGroupPermissionAccess.ReadLocationGroupPermissionRangeByID(locationGroupID))
					.Where(lgp => lgp.Permissions.HasFlag(LocationGroupPermissionType.UseGroup))
					.Select(lgp => lgp.Location)
					.ToList();
				if (locationsFromGroup.Count is 0)
				{
					await RespondAsync("no locations were found in the location group...", ephemeral: true);
					return;
				}

				effectiveLocationFilter = locationsFromGroup;
			}
			if (authorFilter is not null)
			{
				List<AuthorOID> newAuthorsForFilter = [];
				try
				{
					newAuthorsForFilter = authorFilter
						.Split("::")
						.Select(entry => ulong.TryParse(entry, out _)
							? new AuthorOID(ServiceType.Discord, options.DiscordInstance, entry)
							: AuthorOID.Parse(entry))
						.ToList();
				}
				catch
				{
					await RespondAsync("failed to parse the author filter", ephemeral: true);
					return;
				}
				if (!isDev && authorGroup is null && locationGroup is null && locationFilter is null && newAuthorsForFilter.Any(a => a != authorOID))
				{
					await RespondAsync("you cannot filter by authors that are not you without providing a locationgroup, locationfilter, or authorgroup", ephemeral: true);
					return;
				}

				if (authorGroup is not null && !isDev && locationGroup is not null && locationFilter is not null)
				{
					if (newAuthorsForFilter.Except(effectiveAuthorFilter).Any(a => a != authorOID))
					{
						await RespondAsync("you cannot filter by authors that are not you or not in your group unless there is also a locationgroup of locationfilter specified");
						return;
					}
				}
				effectiveAuthorFilter = effectiveAuthorFilter.Concat(newAuthorsForFilter).ToList();
			}
			if (locationFilter is not null)
			{
				List<IObjectOID> newLocationsForFilter = [];
				try
				{
					newLocationsForFilter = locationFilter
						.Split("::")
						.Select(async entry =>
						{
							if (ulong.TryParse(entry, out var id))
							{
								IGuild? guild = Context.Client.GetGuild(id);
								if (guild is not null)
									return DiscordObjectOID.ForServer(options.DiscordInstance, id);

								var channel = Context.Client.GetChannel(id);
								if (channel is IGuildChannel guildChannel)
									guild = guildChannel.Guild;
								if (channel is not null)
									return await objectOIDBuilder.Build(guild, channel, options.DiscordInstance);

								throw new ArgumentException($"No {nameof(IChannel)} or {nameof(IGuild)} found for ID '{id}'.", nameof(locationFilter));
							}
							else
							{
								return objectOIDParser.Parse(entry);
							}
						})
						.Select(task => task.Result)
						.ToList();
				}
				catch
				{
					await RespondAsync("failed to parse the location filter", ephemeral: true);
					return;
				}
				if (!isDev)
				{
					var locations = (await locationAccess.ReadLocationRange(newLocationsForFilter)).ToList();
					var missingLocations = newLocationsForFilter.Where(li => !locations.Any(l => l.ID == li));
					if (missingLocations.Any())
					{
						// Not ephemeral so that they can easily do this contacting
						await RespondAsync("locations without entries in the location database found:\n" +
							$"{missingLocations}\n" +
							"please contact the bot owner in order to fix this.");
						return;
					}
					if (authorGroup is null && authorFilter is null)
					{
						var ownersForLocationsNotInGroup = locations.Where(l => !effectiveLocationFilter.Any(li => li.IsParentOrEqualTo(l.ID))).Select(l => l.Owner).ToList();
						if (ownersForLocationsNotInGroup.Any(lo => lo != authorOID))
						{
							await RespondAsync("you cannot filter by locations that either you do not own or are not in the given locationgroup unless you provide an authorgroup or authorfilter", ephemeral: true);
							return;
						}
					}
					if (authorGroup is null && authorFilter is null && locationGroup is null)
					{
						var locationOwners = locations.Select(l => l.Owner).Distinct().ToList();
						if (locationOwners.Any(lo => lo != authorOID))
						{
							await RespondAsync("you cannot filter by locations that you do not own unless you give an authorgroup, authorfilter, or locationgroup", ephemeral: true);
							return;
						}
					}
				}
				effectiveLocationFilter = effectiveLocationFilter.Concat(newLocationsForFilter).ToList();
			}

			var sentences = await sentenceAccess.ReadSentenceRange(new SentenceFilter(effectiveLocationFilter, effectiveAuthorFilter), searchString, count);
			var listBuilder = ConsoleTableBuilder
				.From(sentences.ToList())
				.WithFormat(ConsoleTableBuilderFormat.Minimal)
				.Export();
			using var stream = new MemoryStream(Encoding.UTF8.GetBytes(listBuilder.ToString()));
			await ReplyAsync("sending you the output in DMs!");
			await Context.User.SendFileAsync(stream, "query results.txt");
		}
	}
}
