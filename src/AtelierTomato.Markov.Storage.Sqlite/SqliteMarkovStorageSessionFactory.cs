using AtelierTomato.Markov.Model;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Storage.Sqlite
{
	public class SqliteMarkovStorageSessionFactory : IMarkovStorageSessionFactory
	{
		private readonly SqliteAccessOptions options;
		private readonly MultiParser<IObjectOID> objectOIDParser;
		public SqliteMarkovStorageSessionFactory(IOptions<SqliteAccessOptions> options, MultiParser<IObjectOID> objectOIDParser)
		{
			this.options = options.Value;
			this.objectOIDParser = objectOIDParser;
		}
		public IMarkovStorageSession CreateSession()
		{
			return new SqliteMarkovStorageSession(options, objectOIDParser);
		}
	}
}
