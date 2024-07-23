namespace AtelierTomato.Markov.Core.Model
{
	public interface IObjectOID
	{
		ServiceType Service { get; }
		string Instance { get; }
		string ToString();
	}
}
