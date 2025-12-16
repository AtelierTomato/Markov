namespace AtelierTomato.Markov.Model
{
	public class LocationGroup
	{
		public ulong ID { get; set; }
		public string Name { get; set; } = string.Empty;
		public LocationGroup(ulong iD, string name)
		{
			ID = iD;
			Name = name;
		}
		public LocationGroup() { }
	};
}
