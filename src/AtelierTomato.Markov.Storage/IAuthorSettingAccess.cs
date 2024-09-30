using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage
{
	public interface IAuthorSettingAccess
	{
		Task<IEnumerable<AuthorSetting>> ReadAllAuthorSettings();
		Task<AuthorSetting?> ReadAuthorSetting(AuthorOID author, IObjectOID queryScope);
		Task<IEnumerable<AuthorSetting>> ReadAuthorSettingRange(IEnumerable<AuthorOID> authors, IEnumerable<IObjectOID> queryScopes);
		Task WriteAuthorSetting(AuthorSetting authorSetting);
		Task WriteAuthorSettingRange(IEnumerable<AuthorSetting> authorSettings);
	}
}
