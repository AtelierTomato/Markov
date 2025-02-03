using Discord.Commands;

namespace AtelierTomato.Markov.Bot.Discord.Service.TypeReaders
{
	public class GuidTypeReader : TypeReader
	{
		public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
		{
			if (Guid.TryParse(input, out var guid))
			{
				return Task.FromResult(TypeReaderResult.FromSuccess(guid));
			}
			return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Invalid GUID format."));
		}
	}
}
