using AtelierTomato.Markov.Data.Model;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Data.Sqlite
{
	public class SqliteSentenceAccess(IOptions<SqliteAccessOptions> options) : ISentenceAccess
	{
		private readonly SqliteAccessOptions options = options.Value;

		public async Task DeleteSentenceRange(SentenceFilter filter)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			await connection.ExecuteAsync($@"
DELETE FROM {nameof(Sentence)} WHERE
( @oid IS NULL OR {nameof(Sentence.OID)} LIKE @oid ) AND
( @author IS NULL OR {nameof(Sentence.Author)} LIKE @author ) AND
( @searchTerm IS NULL OR (' ' || {nameof(Sentence.Text)} || ' ') LIKE '% ' || @searchTerm || ' %'
",
			new
			{
				oid = filter.OID,
				author = filter.Author,
				searchTerm = filter.SearchString
			});
		}

		public Task<Sentence?> ReadNextRandomSentence(List<string> prevList, List<string> previousIDs, SentenceFilter filter)
		{
			throw new NotImplementedException();
		}

		public Task<Sentence?> ReadRandomSentence(SentenceFilter filter)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<Sentence>?> ReadSentenceRange(SentenceFilter filter)
		{
			throw new NotImplementedException();
		}

		public async Task WriteSentence(Sentence sentence) => await WriteSentenceRange([sentence]);

		public async Task WriteSentenceRange(IEnumerable<Sentence> sentenceRange)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();
			await using var transaction = await connection.BeginTransactionAsync();

			foreach (Sentence sentence in sentenceRange)
			{
				await WriteCore(sentence, connection);
			}

			await transaction.CommitAsync();
		}

		private async Task WriteCore(Sentence sentence, SqliteConnection connection)
		{
			await connection.ExecuteAsync($@"
insert into {nameof(Sentence)} ( {nameof(Sentence.OID)}, {nameof(Sentence.Author)}, {nameof(Sentence.Date)}, {nameof(Sentence.Text)} )
Values ( @oid, @author, @date, @text )
on conflict ({nameof(Sentence.OID)}) do update set
{nameof(Sentence.Date)} = excluded.{nameof(Sentence.Date)},
{nameof(Sentence.Text)} = excluded.{nameof(Sentence.Text)}
",
			new
			{
				oid = sentence.OID.ToString(),
				author = sentence.Author.ToString(),
				date = sentence.Date.ToString("o"),
				text = sentence.Text
			}); ;
		}
	}
}
