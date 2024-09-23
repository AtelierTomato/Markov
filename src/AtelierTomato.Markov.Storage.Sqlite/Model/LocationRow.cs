using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Model.ObjectOID.Parser;

namespace AtelierTomato.Markov.Storage.Sqlite.Model
{
	public class LocationRow
	{
		public string ID { get; set; }
		public string Name { get; set; }
		private readonly MultiParser<IObjectOID> ObjectOIDParser = new([new SpecialObjectOIDParser(), new BookObjectOIDParser(), new DiscordObjectOIDParser()]);
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
		public Location ToLocation() => new(ObjectOIDParser.Parse(ID), Name);
	}
}
