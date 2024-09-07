using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Storage.Sqlite.Model;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Storage.Sqlite
{
	public class SqliteSentenceAccess : ISentenceAccess
	{
		private readonly SqliteAccessOptions options;
		public SqliteSentenceAccess(IOptions<SqliteAccessOptions> options)
		{
			this.options = options.Value;
		}

		public async Task DeleteSentenceRange(SentenceFilter filter, string? searchString = null)
		{
			if (filter.OID is null && filter.Author is null && searchString is null)
				throw new ArgumentException("You cannot delete all sentences from the database through this command, at least one part of the filter must have a value.", nameof(filter));

			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			await connection.ExecuteAsync($@"
DELETE FROM {nameof(Sentence)} WHERE
( @oid IS NULL OR {nameof(Sentence.OID)} LIKE @oid || '%' ) AND
( @author IS NULL OR {nameof(Sentence.Author)} LIKE @author || '%' ) AND
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

		public async Task<IEnumerable<Sentence>> ReadNextRandomSentences(int amount, List<string> prevList, List<IObjectOID> previousIDs, SentenceFilter filter, string? keyword = null, IObjectOID? origin = null)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<SentenceRow>($@"
SELECT {nameof(Sentence.OID)}, {nameof(Sentence.Author)}, {nameof(Sentence.Date)}, {nameof(Sentence.Text)} FROM SentenceAfterLinkWithPermission WHERE
( {nameof(AuthorPermission.AllowedScope)} IS NULL OR {nameof(AuthorPermission.AllowedScope)} IS '' OR @origin || ':' LIKE {nameof(AuthorPermission.AllowedScope)} || ':%' ) AND
( @oid IS NULL OR {nameof(Sentence.OID)} LIKE @oid || '%' ) AND
( @author IS NULL OR {nameof(Sentence.Author)} LIKE @author || '%' ) AND
( {nameof(Sentence.OID)} NOT IN @previousIDs ) AND
( ( ' ' || {nameof(Sentence.Text)} || ' ') LIKE '% ' || @prevList || ' %' )
ORDER BY
CASE WHEN @keyword IS NOT NULL AND (' ' || {nameof(Sentence.Text)} || ' ') LIKE '% ' || @keyword || ' %' THEN 1 ELSE 2 END,
RANDOM()
LIMIT @amount
",
			new
			{
				oid = filter.OID?.ToString(),
				author = filter.Author?.ToString(),
				keyword,
				previousIDs = previousIDs.Select(x => x.ToString()),
				prevList = string.Join(' ', prevList),
				amount
			});

			connection.Close();

			return result.Select(s => s.ToSentence());
		}

		public async Task<Sentence?> ReadRandomSentence(SentenceFilter filter, string? keyword = null, IObjectOID? origin = null)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QuerySingleOrDefaultAsync<SentenceRow?>($@"
SELECT {nameof(Sentence.OID)}, {nameof(Sentence.Author)}, {nameof(Sentence.Date)}, {nameof(Sentence.Text)} FROM SentenceAfterLinkWithPermission WHERE
( {nameof(AuthorPermission.AllowedScope)} IS NULL OR {nameof(AuthorPermission.AllowedScope)} IS '' OR @origin || ':' LIKE {nameof(AuthorPermission.AllowedScope)} || ':%' ) AND
( @oid IS NULL OR {nameof(Sentence.OID)} LIKE @oid || '%' ) AND
( @author IS NULL OR {nameof(Sentence.Author)} LIKE @author || '%' )
ORDER BY
CASE WHEN @keyword IS NOT NULL AND (' ' || {nameof(Sentence.Text)} || ' ') LIKE '% ' || @keyword || ' %' THEN 1 ELSE 2 END,
RANDOM()
LIMIT 1
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

			var result = await connection.QueryAsync<SentenceRow>($@"
SELECT {nameof(Sentence.OID)}, {nameof(Sentence.Author)}, {nameof(Sentence.Date)}, {nameof(Sentence.Text)} FROM {nameof(Sentence)} WHERE
( @oid IS NULL OR {nameof(Sentence.OID)} LIKE @oid || '%' ) AND
( @author IS NULL OR {nameof(Sentence.Author)} LIKE @author || '%' ) AND
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
			SentenceRow sentenceRaw = new(sentence);
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
