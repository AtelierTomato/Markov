namespace AtelierTomato.Markov.Model
{
	public class LocationSetting
	{
		public IObjectOID ID { get; set; }
		private List<string> _writeReactions = [];
		public List<string> WriteReactions
		{
			get => _writeReactions;
			set
			{
				if (value.Any(r => r.Contains(':')))
				{
					throw new ArgumentException("Reactions cannot contain colons.", nameof(value));
				}
				_writeReactions = value;
			}
		}
		private List<string> _deleteReactions = [];
		public List<string> DeleteReactions
		{
			get => _deleteReactions;
			set
			{
				if (value.Any(r => r.Contains(':')))
				{
					throw new ArgumentException("Reactions cannot contain colons.", nameof(value));
				}
				_deleteReactions = value;
			}
		}
		private List<string> _failReactions = [];
		public List<string> FailReactions
		{
			get => _failReactions;
			set
			{
				if (value.Any(r => r.Contains(':')))
				{
					throw new ArgumentException("Reactions cannot contain colons.", nameof(value));
				}
				_failReactions = value;
			}
		}
		public bool? GlobalAllowed { get; set; }
		public Guid? LocationGroup { get; set; }
		public LocationSetting(IObjectOID ID, List<string> writeReactions, List<string> deleteReactions, List<string> failReactions, bool? globalAllowed, Guid? locationGroup)
		{
			this.ID = ID;
			WriteReactions = writeReactions;
			DeleteReactions = deleteReactions;
			FailReactions = failReactions;
			GlobalAllowed = globalAllowed;
			LocationGroup = locationGroup;
		}
	}
}
