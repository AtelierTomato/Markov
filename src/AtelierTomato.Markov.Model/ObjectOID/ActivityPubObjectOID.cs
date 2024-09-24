using System.Globalization;
using System.Text.RegularExpressions;

namespace AtelierTomato.Markov.Model.ObjectOID
{
	public class ActivityPubObjectOID : IObjectOID
	{
		public ServiceType Service { get; } = ServiceType.ActivityPub;
		public string Instance { get; set; }
		public string? ID { get; set; }
		public int? Sentence { get; set; }
		public ActivityPubObjectOID(string instance, string? ID = null, int? sentence = null)
		{
			Instance = instance;
			this.ID = ID;
			Sentence = sentence;
		}
		public static ActivityPubObjectOID ForInstance(string instance)
			=> new(instance);
		public static ActivityPubObjectOID ForID(string instance, string ID)
			=> new(instance, ID);
		public static ActivityPubObjectOID ForSentence(string instance, string ID, int sentence)
			=> new(instance, ID, sentence);
		public static ActivityPubObjectOID Parse(string OID)
		{
			if (string.IsNullOrWhiteSpace(OID))
			{
				throw new ArgumentException("The OID given is empty.", nameof(OID));
			}

			Regex activityPubOIDRegex = OIDPattern.Generate([nameof(ServiceType), nameof(Instance), nameof(ID), nameof(Sentence)]);

			var match = activityPubOIDRegex.Match(OID);

			if (!match.Success)
				throw new ArgumentException("The OID given is not a valid ActivityPubObjectOID");

			if (match.Groups[nameof(ServiceType)].Value != ServiceType.ActivityPub.ToString())
				throw new ArgumentException("The OID given is not an ActivityPubObjectOID, as it does not begin with ActivityPub.", nameof(OID));

			var instance = OIDEscapement.Unescape(match.Groups[nameof(Instance)].Value);

			if (!match.Groups[nameof(ID)].Success)
			{
				return ForInstance(instance);
			}
			var id = OIDEscapement.Unescape(match.Groups[nameof(ID)].Value);

			if (!match.Groups[nameof(Sentence)].Success)
			{
				return ForID(instance, id);
			}
			if (!int.TryParse(match.Groups[nameof(Sentence)].Value, out var sentence))
				throw new ArgumentException("The part of the ActivityPubObjectOID corresponding to the sentence was not able to be parsed into an integer value.", nameof(OID));

			return ForSentence(instance, id, sentence);
		}

		public override string ToString()
		{
			var oidBuilder = new OIDBuilder(ServiceType.ActivityPub);
			oidBuilder.Append(Instance);
			if (ID is not null)
			{
				oidBuilder.Append(ID);
			}
			else
			{
				return oidBuilder.Build();
			}
			if (Sentence.HasValue)
			{
				oidBuilder.Append(Sentence.Value.ToString(CultureInfo.InvariantCulture));
			}
			else
			{
				return oidBuilder.Build();
			}
			return oidBuilder.Build();
		}
		public ActivityPubObjectOID WithSentence(int sentence)
		{
			if (ID is null)
			{
				throw new InvalidOperationException("An ActivityPubObjectOID cannot be returned with a Sentence if there is no value in ID.");
			}
			return new ActivityPubObjectOID(Instance, ID, sentence);
		}
	}
}
