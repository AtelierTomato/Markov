using AtelierTomato.Markov.Model;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Storage.Sqlite
{
	public class SqliteAuthorAccess : IAuthorAccess
	{
		private readonly SqliteAccessOptions options;
		public SqliteAuthorAccess(IOptions<SqliteAccessOptions> options)
		{
			this.options = options.Value;
		}

		public async Task<Author?> ReadAuthor(AuthorOID ID) => (await ReadAuthorRange([ID])).FirstOrDefault();
		public async Task<IEnumerable<Author>> ReadAuthorRange(IEnumerable<AuthorOID> IDs)
		{
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();

			var result = await connection.QueryAsync<AuthorRow>($@"SELECT {nameof(Author.ID)}, {nameof(Author.Name)} FROM {nameof(Author)} WHERE {nameof(Author.ID)} IN @ids",
				new
				{
					ids = IDs.Select(i => i.ToString())
				});

			connection.Close();

			return result.Select(a => a.ToAuthor());
		}

		public async Task WriteAuthor(Author author) => await WriteAuthorRange([author]);
		public async Task WriteAuthorRange(IEnumerable<Author> authors)
		{
			var authorRows = authors.Select(a => new AuthorRow(a));
			await using var connection = new SqliteConnection(options.ConnectionString);
			connection.Open();
			await using var transaction = await connection.BeginTransactionAsync();

			foreach (AuthorRow authorRow in authorRows)
			{
				await connection.ExecuteAsync($@"
INSERT INTO {nameof(Author)} ( {nameof(Author.ID)}, {nameof(Author.Name)} )
Values ( @id, @name )
ON CONFLICT ({nameof(Author.ID)}) DO UPDATE SET
{nameof(Author.Name)} = excluded.{nameof(Author.Name)}
",
				new
				{
					id = authorRow.ID,
					name = authorRow.Name
				});
			}

			await transaction.CommitAsync();
		}
	}
}
