using AtelierTomato.Markov.Data.Model;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Data.Sqlite
{
	public class SqliteSentenceAccess(IOptions<SqliteAccessOptions> options) : ISentenceAccess
	{
		private readonly SqliteAccessOptions options = options.Value;

		public async Task DeleteSentenceRange(SentenceFilter filter, string? searchString = null)
		{
			if (filter.OID is null && filter.Author is null && searchString is null)
				throw new ArgumentException("You cannot delete all sentences from the database through this command, at least one part of the filter must have a value.", nameof(filter));

			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			await connection.ExecuteAsync($@"
DELETE FROM {nameof(Sentence)} WHERE
( @oid IS NULL OR {nameof(Sentence.OID)} LIKE @oid ) AND
( @author IS NULL OR {nameof(Sentence.Author)} LIKE @author ) AND
( @searchString IS NULL OR (' ' || {nameof(Sentence.Text)} || ' ') LIKE '% ' || @searchString || ' %' )
",
			new
			{
				oid = filter.OID?.ToString(),
				author = filter.Author?.ToString(),
				searchString
			});

			connection.Close();
		}

		public async Task<Sentence?> ReadNextRandomSentence(List<string> prevList, List<IObjectOID> previousIDs, SentenceFilter filter, string? keyword = null)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QuerySingleOrDefaultAsync<SentenceRaw?>($@"
SELECT * FROM {nameof(Sentence)} WHERE
( @oid IS NULL OR {nameof(Sentence.OID)} LIKE @oid ) AND
( @author IS NULL OR {nameof(Sentence.Author)} LIKE @author ) AND
( @keyword IS NULL OR (' ' || {nameof(Sentence.Text)} || ' ') LIKE '% ' || @keyword || ' %' ) AND
( {nameof(Sentence.OID)} NOT IN @previousIDs ) AND
( ( ' ' || {nameof(Sentence.Text)} || ' ') LIKE '% ' || @prevList || ' %' )
ORDER BY RANDOM() LIMIT 1
",
			new
			{
				oid = filter.OID?.ToString(),
				author = filter.Author?.ToString(),
				keyword,
				previousIDs = previousIDs.Select(x => x.ToString()),
				prevList = string.Join(' ', prevList)
			});

			connection.Close();

			return result?.ToSentence();
		}

		public async Task<Sentence?> ReadRandomSentence(SentenceFilter filter, string? keyword = null)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QuerySingleOrDefaultAsync<SentenceRaw?>($@"
SELECT * FROM {nameof(Sentence)} WHERE
( @oid IS NULL OR {nameof(Sentence.OID)} LIKE @oid ) AND
( @author IS NULL OR {nameof(Sentence.Author)} LIKE @author ) AND
( @keyword IS NULL OR (' ' || {nameof(Sentence.Text)} || ' ') LIKE '% ' || @keyword || ' %' )
ORDER BY RANDOM() LIMIT 1
",
			new
			{
				oid = filter.OID?.ToString(),
				author = filter.Author?.ToString(),
				keyword
			});

			connection.Close();

			return result?.ToSentence();
		}

		public async Task<IEnumerable<Sentence>> ReadSentenceRange(SentenceFilter filter, string? searchString = null)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<SentenceRaw>($@"
SELECT * FROM {nameof(Sentence)} WHERE
( @oid IS NULL OR {nameof(Sentence.OID)} LIKE @oid ) AND
( @author IS NULL OR {nameof(Sentence.Author)} LIKE @author ) AND
( @searchString IS NULL OR (' ' || {nameof(Sentence.Text)} || ' ') LIKE '% ' || @searchString || ' %' )
",
			new
			{
				oid = filter.OID?.ToString(),
				author = filter.Author?.ToString(),
				searchString
			});

			connection.Close();
			return result.Select(s => s.ToSentence());
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
			SentenceRaw sentenceRaw = new(sentence);
			await connection.ExecuteAsync($@"
insert into {nameof(Sentence)} ( {nameof(Sentence.OID)}, {nameof(Sentence.Author)}, {nameof(Sentence.Date)}, {nameof(Sentence.Text)} )
Values ( @oid, @author, @date, @text )
on conflict ({nameof(Sentence.OID)}) do update set
{nameof(Sentence.Date)} = excluded.{nameof(Sentence.Date)},
{nameof(Sentence.Text)} = excluded.{nameof(Sentence.Text)}
",
			new
			{
				oid = sentenceRaw.OID,
				author = sentenceRaw.Author,
				date = sentenceRaw.Date,
				text = sentenceRaw.Text
			}); ;
		}
	}
}
