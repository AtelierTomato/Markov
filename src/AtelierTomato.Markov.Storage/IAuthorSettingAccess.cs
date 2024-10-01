using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage
{
	public interface IAuthorSettingAccess
	{
		Task<IEnumerable<AuthorSetting>> ReadAllAuthorSettings();
		Task<AuthorSetting?> ReadAuthorSetting(AuthorOID author, IObjectOID location);
		Task<IEnumerable<AuthorSetting>> ReadAuthorSettingRangeByAuthor(AuthorOID author);
		Task<IEnumerable<AuthorSetting>> ReadAuthorSettingRangeByLocation(IObjectOID location);
		Task WriteAuthorSetting(AuthorSetting authorSetting);
		Task DeleteAuthorSetting(AuthorOID author, IObjectOID location);
		Task DeleteAuthorSettingRangeByAuthor(AuthorOID author);
		Task DeleteAuthorSettingRangeByLocation(IObjectOID location);
	}
}
