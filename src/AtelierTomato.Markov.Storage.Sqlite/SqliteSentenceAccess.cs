using System.Globalization;
using System.Text;
using AtelierTomato.Markov.Model;
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
			if (filter.OIDs.ToList() is null or [] && filter.Authors.ToList() is null or [] && searchString is null)
				throw new ArgumentException("You cannot delete all sentences from the database through this command, at least one part of the filter must have a value.", nameof(filter));

			var sbOIDs = GenerateOIDStringBuilder(filter);

			IEnumerable<string>? authors = null;
			if (filter.Authors is not null && filter.Authors.Any())
			{
				authors = filter.Authors.Select(a => a.ToString());
			}

			await using var connection = new SqliteConnection(options.ConnectionString);

			connection.Open();

			await connection.ExecuteAsync($@"
DELETE FROM {nameof(Sentence)} WHERE
{sbOIDs}
( @authors IS NULL OR {nameof(Sentence.Author)} IN @authors ) AND
( @searchString IS NULL OR (' ' || {nameof(Sentence.Text)} || ' ') LIKE '% ' || @searchString || ' %' )
",
			new
			{
				authors,
				searchString
			});

			connection.Close();
		}

		public async Task<IEnumerable<Sentence>> ReadNextRandomSentences(int amount, List<string> prevList, List<IObjectOID> previousIDs, SentenceFilter filter, string? keyword = null)
		{
			var sbOIDs = GenerateOIDStringBuilder(filter);

			IEnumerable<string>? authors = null;
			if (filter.Authors is not null && filter.Authors.Any())
			{
				authors = filter.Authors.Select(a => a.ToString());
			}

			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<SentenceRaw>($@"
SELECT {nameof(Sentence.OID)}, {nameof(Sentence.Author)}, {nameof(Sentence.Date)}, {nameof(Sentence.Text)} FROM {nameof(Sentence)} WHERE
{sbOIDs}
( @authors IS NULL OR {nameof(Sentence.Author)} IN @authors ) AND
( {nameof(Sentence.OID)} NOT IN @previousIDs ) AND
( ( ' ' || {nameof(Sentence.Text)} || ' ') LIKE '% ' || @prevList || ' %' )
ORDER BY
CASE WHEN @keyword IS NOT NULL AND (' ' || {nameof(Sentence.Text)} || ' ') LIKE '% ' || @keyword || ' %' THEN 1 ELSE 2 END,
RANDOM()
LIMIT @amount
",
			new
			{
				authors,
				keyword,
				previousIDs = previousIDs.Select(x => x.ToString()),
				prevList = string.Join(' ', prevList),
				amount
			});

			connection.Close();

			return result.Select(s => s.ToSentence());
		}

		public async Task<Sentence?> ReadRandomSentence(SentenceFilter filter, string? keyword = null)
		{
			var sbOIDs = GenerateOIDStringBuilder(filter);

			IEnumerable<string>? authors = null;
			if (filter.Authors is not null && filter.Authors.Any())
			{
				authors = filter.Authors.Select(a => a.ToString());
			}

			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QuerySingleOrDefaultAsync<SentenceRaw?>($@"
SELECT {nameof(Sentence.OID)}, {nameof(Sentence.Author)}, {nameof(Sentence.Date)}, {nameof(Sentence.Text)} FROM {nameof(Sentence)} WHERE
{sbOIDs}
( @authors IS NULL OR {nameof(Sentence.Author)} IN @authors )
ORDER BY
CASE WHEN @keyword IS NOT NULL AND (' ' || {nameof(Sentence.Text)} || ' ') LIKE '% ' || @keyword || ' %' THEN 1 ELSE 2 END,
RANDOM()
LIMIT 1
",
			new
			{
				authors,
				keyword
			});

			connection.Close();

			return result?.ToSentence();
		}

		public async Task<IEnumerable<Sentence>> ReadSentenceRange(SentenceFilter filter, string? searchString = null)
		{
			var sbOIDs = GenerateOIDStringBuilder(filter);

			IEnumerable<string>? authors = null;
			if (filter.Authors is not null && filter.Authors.Any())
			{
				authors = filter.Authors.Select(a => a.ToString());
			}

			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<SentenceRaw>($@"
SELECT {nameof(Sentence.OID)}, {nameof(Sentence.Author)}, {nameof(Sentence.Date)}, {nameof(Sentence.Text)} FROM {nameof(Sentence)} WHERE
{sbOIDs}
( @authors IS NULL OR {nameof(Sentence.Author)} IN @authors ) AND
( @searchString IS NULL OR (' ' || {nameof(Sentence.Text)} || ' ') LIKE '% ' || @searchString || ' %' )
",
			new
			{
				authors,
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
			});
		}

		private static StringBuilder GenerateOIDStringBuilder(SentenceFilter filter)
		{
			var sbOIDs = new StringBuilder();
			if (filter.OIDs is not null && filter.OIDs.Any())
			{
				sbOIDs.Append('(');
				var oids = filter.OIDs.ToList();
				for (int i = 0; i < oids.Count; i++)
				{
					if (i > 0)
					{
						sbOIDs.Append(" OR");
					}
					sbOIDs.Append(CultureInfo.InvariantCulture, $" {nameof(Sentence.OID)} LIKE '{oids[i]}%'");
				}
				sbOIDs.Append(" ) AND" + Environment.NewLine);
			}

			return sbOIDs;
		}
	}
}
