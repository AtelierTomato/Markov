namespace AtelierTomato.Markov.Model
{
	public interface IObjectOID
	{
		ServiceType Service { get; }
		string? Instance { get; }
		IObjectOID Base();
		string ToString();
	}
}
