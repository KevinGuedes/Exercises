using Microsoft.Data.Sqlite;

namespace Questao5.Infrastructure.Sqlite;

public sealed class SqlConnectionManager(DatabaseConfig dbConfig) : ISqlConnectionManager
{
    private readonly SqliteConnection _connection = new(dbConfig.ConnectionString);

    public SqliteConnection GetConnection() => _connection;
}
