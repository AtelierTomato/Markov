using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage.Sqlite.Model
{
	public class LocationGroupRow
	{
		public string ID { get; set; }
		public string Name { get; set; }
		public LocationGroupRow(string ID, string name)
		{
			this.ID = ID;
			Name = name;
		}
		public LocationGroupRow(LocationGroup locationGroup)
		{
			ID = locationGroup.ID.ToString();
			Name = locationGroup.Name;
		}
		public LocationGroup ToLocationGroup() => new(Guid.Parse(ID), Name);
	}
}
