using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage.Sqlite
{
	public class AuthorRow
	{
		public string ID { get; set; }
		public string Name { get; set; }
		public AuthorRow(string ID, string name)
		{
			this.ID = ID;
			Name = name;
		}
		public AuthorRow(Author author)
		{
			ID = author.ID.ToString();
			Name = author.Name;
		}
		public Author ToAuthor() => new(AuthorOID.Parse(ID), Name);
	}
}
