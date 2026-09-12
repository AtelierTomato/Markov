using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage.Sqlite.Model
{
	public class LocationRow
	{
		public string ID { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string Owner { get; set; } = string.Empty;
		public LocationRow() { }
		public LocationRow(string ID, string name, string owner)
		{
			this.ID = ID;
			Name = name;
			Owner = owner;
		}
		public LocationRow(Location location)
		{
			ID = location.ID.ToString();
			Name = location.Name;
			Owner = location.Owner.ToString();
		}
		public Location ToLocation(MultiParser<IObjectOID> objectOIDParser) => new(objectOIDParser.Parse(ID), Name, AuthorOID.Parse(Owner));
	}
}
