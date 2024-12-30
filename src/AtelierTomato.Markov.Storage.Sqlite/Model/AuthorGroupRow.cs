using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage.Sqlite.Model
{
	public class AuthorGroupRow
	{
		public string ID { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public AuthorGroupRow() { }
		public AuthorGroupRow(string ID, string name)
		{
			this.ID = ID;
			Name = name;
		}
		public AuthorGroupRow(AuthorGroup authorGroup)
		{
			ID = authorGroup.ID.ToString();
			Name = authorGroup.Name;
		}
		public AuthorGroup ToAuthorGroup() => new(Guid.Parse(ID), Name);
	}
}
