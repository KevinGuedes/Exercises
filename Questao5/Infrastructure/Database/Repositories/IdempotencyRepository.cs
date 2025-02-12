using Dapper;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces.Repositories;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.Infrastructure.Database.Repositories;

public sealed class IdempotencyRepository(ISqlConnectionManager sqlConnectionManager) : IIdempotencyRepository
{
    private readonly ISqlConnectionManager _sqlConnectionManager = sqlConnectionManager;

    public async Task InsertAsync(Idempotency idempotency)
    {
        const string sql = @"
            INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado)
            VALUES (@Key, @Request, @Result)
        ";

        using var connection = _sqlConnectionManager.GetConnection();
        await connection.ExecuteAsync(sql, idempotency);
    }

    public Task<Idempotency?> GetByKeyAsync(Guid key)
    {
        const string sql = @"
            SELECT 
                chave_idempotencia as Key, 
                requisicao as Request, 
                resultado as Result
            FROM 
                idempotencia
            WHERE 
                chave_idempotencia = @Key
        ";

        using var connection = _sqlConnectionManager.GetConnection();
        return connection.QueryFirstOrDefaultAsync<Idempotency?>(sql, new { Key = key });
    }
}
