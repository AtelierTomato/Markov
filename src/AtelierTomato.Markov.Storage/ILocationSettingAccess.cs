using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage
{
	public interface ILocationSettingAccess
	{
		Task WriteLocationSetting(LocationSetting locationSetting);
		Task WriteLocationSettingRange(IEnumerable<LocationSetting> locationSettings);
		Task<LocationSetting?> ReadLocationSetting(IObjectOID ID);
		Task<IEnumerable<LocationSetting>> ReadLocationSettingHierarchy(IObjectOID ID);
		Task<IEnumerable<LocationSetting>> ReadLocationSettingRangeByBaseLocation(IObjectOID ID);
		Task<IEnumerable<LocationSetting>> ReadAllLocationSettings();
		Task DeleteLocationSetting(IObjectOID ID);
	}
}
