using AtelierTomato.Markov.Model;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Storage.Sqlite
{
	public class SqliteWordStatisticAccess : IWordStatisticAccess
	{
		private readonly SqliteAccessOptions options;
		public SqliteWordStatisticAccess(IOptions<SqliteAccessOptions> options)
		{
			this.options = options.Value;
		}

		public async Task<WordStatistic> ReadWordStatistic(string word)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QuerySingleOrDefaultAsync<WordStatistic?>($@"
select * from {nameof(WordStatistic)} where {nameof(WordStatistic.Name)} = @name
",
			new { name = word });

			connection.Close();

			return result ?? new WordStatistic(word, Appearances: 0);
		}

		public async Task<IEnumerable<WordStatistic>> ReadWordStatisticRange(IEnumerable<string> words)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<WordStatistic>($@"
select * from {nameof(WordStatistic)} where {nameof(WordStatistic.Name)} in @names
",
			new { names = words });

			connection.Close();

			return result;
		}

		private static async Task WriteCore(WordStatistic wordStatistic, SqliteConnection connection)
		{
			await connection.ExecuteAsync($@"
insert into {nameof(WordStatistic)} ( {nameof(WordStatistic.Name)}, {nameof(WordStatistic.Appearances)} )
Values ( @name, @appearances )
on conflict ({nameof(WordStatistic.Name)}) do update set
{nameof(WordStatistic.Appearances)} = excluded.{nameof(WordStatistic.Appearances)}
",
			new
			{
				name = wordStatistic.Name,
				appearances = wordStatistic.Appearances
			});
		}

		public async Task WriteWordStatistic(WordStatistic wordStatistic) => await WriteWordStatisticRange([wordStatistic]);

		public async Task WriteWordStatisticRange(IEnumerable<WordStatistic> wordStatistics)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();
			await using var transaction = await connection.BeginTransactionAsync();

			foreach (WordStatistic wordStatistic in wordStatistics)
			{
				await WriteCore(wordStatistic, connection);
			}

			await transaction.CommitAsync();
		}
	}
}
