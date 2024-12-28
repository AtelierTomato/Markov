using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage.Sqlite.Model
{
	public class LocationSettingRow
	{
		public string ID { get; set; }
		public string WriteReactions { get; set; }
		public string DeleteReactions { get; set; }
		public string FailReactions { get; set; }
		public int? GlobalAllowed { get; set; }
		public string? LocationGroup { get; set; }
		public LocationSettingRow(string ID, string writeReactions, string deleteReactions, string failReactions, int? globalAllowed, string? locationGroup)
		{
			this.ID = ID;
			WriteReactions = writeReactions;
			DeleteReactions = deleteReactions;
			FailReactions = failReactions;
			GlobalAllowed = globalAllowed;
			LocationGroup = locationGroup;
		}
		public LocationSettingRow(LocationSetting locationSetting)
		{
			ID = locationSetting.ID.ToString();
			WriteReactions = string.Join(':', locationSetting.WriteReactions);
			DeleteReactions = string.Join(':', locationSetting.DeleteReactions);
			FailReactions = string.Join(':', locationSetting.FailReactions);
			if (locationSetting.GlobalAllowed is not null)
				GlobalAllowed = Convert.ToInt32((bool)locationSetting.GlobalAllowed);
			else GlobalAllowed = null;
			LocationGroup = locationSetting.LocationGroup.ToString();
		}
		public LocationSetting ToLocationSetting(MultiParser<IObjectOID> objectOIDParser)
		{
			IObjectOID id = objectOIDParser.Parse(ID);
			List<string> writeReactions = WriteReactions.Split(':').ToList();
			List<string> deleteReactions = DeleteReactions.Split(':').ToList();
			List<string> failReactions = FailReactions.Split(':').ToList();
			bool? globalAllowed;
			if (GlobalAllowed is not null)
				globalAllowed = Convert.ToBoolean((int)GlobalAllowed);
			else globalAllowed = null;
			Guid? locationGroup;
			if (LocationGroup is not null)
				locationGroup = new Guid(LocationGroup);
			else locationGroup = null;
			return new(id, writeReactions, deleteReactions, failReactions, globalAllowed, locationGroup);
		}
	}
}
