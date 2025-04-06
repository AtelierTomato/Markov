using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Model.ObjectOID;
using AtelierTomato.Markov.Model.ObjectOID.LocationTypes;
using AtelierTomato.Markov.Service.Discord;
using AtelierTomato.Markov.Storage;
using Discord.Interactions;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Bot.Discord.Core.CommandModules
{
	public class AuthorRetortConfigModule : InteractionModuleBase<SocketInteractionContext>
	{
		private readonly IAuthorRetortConfigAccess authorRetortConfigAccess;
		private readonly IAuthorGroupPermissionAccess authorGroupPermissionAccess;
		private readonly ILocationGroupPermissionAccess locationGroupPermissionAccess;
		private readonly DiscordObjectOIDBuilder objectOIDBuilder;
		private readonly MultiParser<IObjectOID> objectOIDParser;
		private readonly DiscordBotOptions options;
		public AuthorRetortConfigModule(IAuthorRetortConfigAccess authorRetortConfigAccess, IAuthorGroupPermissionAccess authorGroupPermissionAccess, ILocationGroupPermissionAccess locationGroupPermissionAccess, DiscordObjectOIDBuilder objectOIDBuilder, MultiParser<IObjectOID> objectOIDParser, IOptions<DiscordBotOptions> options)
		{
			this.authorRetortConfigAccess = authorRetortConfigAccess;
			this.authorGroupPermissionAccess = authorGroupPermissionAccess;
			this.locationGroupPermissionAccess = locationGroupPermissionAccess;
			this.objectOIDBuilder = objectOIDBuilder;
			this.objectOIDParser = objectOIDParser;
			this.options = options.Value;
		}

		[SlashCommand("retort", "Adjust settings for retorts.")]
		public async Task RetortCommand(
			[Summary("location", "Location that the setting will be used for, can be relative or an exact OID")] string locationParam,
			[Summary("displayoption", "Display option (mimic or normal")] string displayOptionParam = "Normal",
			[Summary("authorfilter", "Triple colon (:::) separated list of authors")] string? authorFilterParam = null,
			[Summary("locationfilter", "Triple colon (:::) separated list of locations")] string? locationFilterParam = null,
			[Summary("authorgroup", "ID for the author group")] string? authorGroupParam = null,
			[Summary("locationgroup", "ID for the location group")] string? locationGroupParam = null,
			[Summary("keyword", "Keyword to use when generating")] string? keyword = null,
			[Summary("firstword", "First word to use when generating")] string? firstWord = null
		)
		{
			var author = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var baseLocation = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			IObjectOID location = new SpecialObjectOID(Model.ObjectOID.Types.SpecialObjectOIDType.Invalid);
			try
			{
				location = objectOIDParser.Parse(locationParam);
			}
			catch
			{
				if (Enum.TryParse<DiscordLocationType>(locationParam, true, out var locationDepth))
				{
					try
					{
						if (locationDepth is DiscordLocationType.Message or DiscordLocationType.Sentence)
						{
							await RespondAsync($"the location for a retort setting cannot be {locationDepth}!", ephemeral: true);
							return;
						}
						else if (locationDepth is DiscordLocationType.Global)
						{
							locationDepth = DiscordLocationType.Discord;    // They're functionally the same and we didn't implement making Location null
						}
						location = baseLocation.ForLocationType(locationDepth)!;
					}
					catch (Exception ex)
					{
						await RespondAsync(ex.Message, ephemeral: true);
						return;
					}
				}
			}
			if (location == new SpecialObjectOID(Model.ObjectOID.Types.SpecialObjectOIDType.Invalid))
			{
				await RespondAsync("a valid location was not provided.", ephemeral: true);
				return;
			}
			if (!Enum.TryParse<DisplayOptionType>(displayOptionParam, true, out var displayOption))
			{
				await RespondAsync("the display option was not valid", ephemeral: true);
				return;
			}
			List<IObjectOID> locationsForFilter = [];
			if (locationFilterParam is not null)
			{
				var locationList = locationFilterParam.Split(":::");
				foreach (var l in locationList)
				{
					try
					{
						locationsForFilter = locationsForFilter.Append(objectOIDParser.Parse(l)).ToList();
					}
					catch
					{
						if (Enum.TryParse<DiscordLocationType>(l, true, out var locationDepth))
						{
							if (locationDepth is DiscordLocationType.Global)
							{
								locationsForFilter = []; // Global means null
								break;
							}
							try
							{
								if (locationDepth is DiscordLocationType.Message or DiscordLocationType.Sentence)
								{
									await RespondAsync($"the location for a retort setting cannot be {locationDepth}!", ephemeral: true);
									return;
								}
								else if (locationDepth is DiscordLocationType.Global)
								{
									locationDepth = DiscordLocationType.Discord;    // They're functionally the same and we didn't implement making Location null
								}
								locationsForFilter = locationsForFilter.Append(baseLocation.ForLocationType(locationDepth)!).ToList();
							}
							catch
							{
								await RespondAsync("one or more of the locations for the filter was invalid.", ephemeral: true);
								return;
							}
						}
					}
				}
			}
			List<AuthorOID> authorsForFilter = [];
			if (authorFilterParam is not null)
			{
				var authorList = authorFilterParam.Split(":::");
				foreach (var a in authorList)
				{
					try
					{
						authorsForFilter = authorsForFilter.Append(AuthorOID.Parse(a)).ToList();
					}
					catch
					{
						if (ulong.TryParse(a, out var authorID))
						{
							authorsForFilter = authorsForFilter.Append(new AuthorOID(ServiceType.Discord, options.DiscordInstance, authorID.ToString())).ToList();
						}
						else
						{
							await RespondAsync("one or more of the authors for the filter was invalid.", ephemeral: true);
							return;
						}
					}
				}
			}
			ulong? authorGroup = null;
			if (authorGroupParam is not null)
			{
				if (ulong.TryParse(authorGroupParam, out var temp))
				{
					authorGroup = temp;
				}
				else
				{
					await RespondAsync("authorgroup was not a valid integer", ephemeral: true);
					return;
				}
			}
			ulong? locationGroup = null;
			if (locationGroupParam is not null)
			{
				if (ulong.TryParse(locationGroupParam, out var temp))
				{
					locationGroup = temp;
				}
				else
				{
					await RespondAsync("authorgroup was not a valid integer", ephemeral: true);
					return;
				}
			}

			var authorRetortConfig = new AuthorRetortConfig(author, location, displayOption, new(locationsForFilter, authorsForFilter), authorGroup, locationGroup, keyword, firstWord);
			await authorRetortConfigAccess.WriteAuthorRetortConfig(authorRetortConfig);
			await RespondAsync("set your author retort config!", ephemeral: true);
		}
	}
}
