namespace AtelierTomato.Markov.Storage
{
	public interface IMarkovStorageSessionFactory
	{
		IMarkovStorageSession CreateSession();
	}
}
