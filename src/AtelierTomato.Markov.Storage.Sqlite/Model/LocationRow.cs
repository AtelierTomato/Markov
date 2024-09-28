using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage.Sqlite.Model
{
	public class LocationRow
	{
		public string ID { get; set; }
		public string Name { get; set; }
		public LocationRow(string ID, string name)
		{
			this.ID = ID;
			Name = name;
		}
		public LocationRow(Location location)
		{
			ID = location.ID.ToString();
			Name = location.Name;
		}
		public Location ToLocation(MultiParser<IObjectOID> objectOIDParser) => new(objectOIDParser.Parse(ID), Name);
	}
}
