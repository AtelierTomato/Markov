using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage
{
	public interface IUserGroupAccess
	{
		Task<UserGroup> ReadUserGroup(string ID, AuthorOID author);
		Task<IEnumerable<UserGroup>> ReadUserGroupRangeByID(string ID);
		Task<IEnumerable<UserGroup>> ReadUserGroupRangeByAuthor(AuthorOID author);
		Task WriteUserGroup(UserGroup userGroup);
		Task WriteUserGroupRange(IEnumerable<UserGroup> userGroups);
	}
}
