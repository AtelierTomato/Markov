namespace AtelierTomato.Markov.Data.Model
{
	public interface IObjectOID
	{
		ServiceType Service { get; }
		string Instance { get; }
		string ToString();
	}
}
