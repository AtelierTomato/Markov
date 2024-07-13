using AtelierTomato.Markov.Data.Model;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Data.Sqlite
{
	public class SqliteWordAccess : IWordAccess
	{
		private readonly SqliteAccessOptions options;
		public SqliteWordAccess(IOptions<SqliteAccessOptions> options)
		{
			this.options = options.Value;
		}

		public async Task<Word> ReadWord(string name)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QuerySingleOrDefaultAsync<Word?>($@"
select * from {nameof(Word)} where {nameof(Word.Name)} = @name
",
			new { name });

			connection.Close();

			return result ?? new Word(name, Appearances: 0);
		}

		public async Task<IEnumerable<Word>> ReadWordRange(IEnumerable<string> names)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<Word>($@"
select * from {nameof(Word)} where {nameof(Word.Name)} in @names
",
			new { names });

			connection.Close();

			return result;
		}

		private static async Task WriteCore(Word word, SqliteConnection connection)
		{
			await connection.ExecuteAsync($@"
insert into {nameof(Word)} ( {nameof(Word.Name)}, {nameof(Word.Appearances)} )
Values ( @name, @appearances )
on conflict ({nameof(Word.Name)}) do update set
{nameof(Word.Appearances)} = excluded.{nameof(Word.Appearances)}
",
			new
			{
				name = word.Name,
				appearances = word.Appearances
			});
		}

		public async Task WriteWord(Word word) => await WriteWordRange([word]);

		public async Task WriteWordRange(IEnumerable<Word> words)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();
			await using var transaction = await connection.BeginTransactionAsync();

			foreach (Word word in words)
			{
				await WriteCore(word, connection);
			}

			await transaction.CommitAsync();
		}
	}
}
