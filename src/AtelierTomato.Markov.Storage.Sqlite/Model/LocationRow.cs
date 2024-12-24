using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage.Sqlite.Model
{
	public class LocationRow
	{
		public string ID { get; set; }
		public string Name { get; set; }
		public string Owner { get; set; }
		public int GlobalEnabled { get; set; }
		public LocationRow(string ID, string name, string owner, int globalEnabled)
		{
			this.ID = ID;
			Name = name;
			Owner = owner;
			GlobalEnabled = globalEnabled;
		}
		public LocationRow(Location location)
		{
			ID = location.ID.ToString();
			Name = location.Name;
			Owner = location.Owner.ToString();
			GlobalEnabled = Convert.ToInt32(location.GlobalEnabled);
		}
		public Location ToLocation(MultiParser<IObjectOID> objectOIDParser) => new(objectOIDParser.Parse(ID), Name, AuthorOID.Parse(Owner), Convert.ToBoolean(GlobalEnabled));
	}
}
