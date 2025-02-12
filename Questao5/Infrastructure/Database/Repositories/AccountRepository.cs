using Dapper;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.Infrastructure.Database.Repositories;

public sealed class AccountRepository(ISqlConnectionManager sqlConnectionManager) : IAccountRepository
{
    private readonly ISqlConnectionManager _sqlConnectionManager = sqlConnectionManager;

    public Task<Account?> GetByIdAsync(Guid accountId)
    {
        const string sql = @"
            SELECT 
                idcontacorrente AS Id, 
                nome AS Name, 
                ativo AS IsActive,
                numero AS Number
            FROM 
                contacorrente
            WHERE 
                idcontacorrente = @AccountId
        ";

        using var connection = _sqlConnectionManager.GetConnection();
        return connection.QueryFirstOrDefaultAsync<Account>(sql, new { AccountId = accountId.ToString() });
    }

    public Task<decimal> GetBalanceAsync(Guid accountId)
    {
        const string sql = @"
            SELECT 
                COALESCE(SUM(CASE WHEN tipomovimento = 'C' THEN valor ELSE 0 END), 0) -
                COALESCE(SUM(CASE WHEN tipomovimento = 'D' THEN valor ELSE 0 END), 0) 
            FROM movimento
            WHERE idcontacorrente = @AccountId
        ";

        using var connection = _sqlConnectionManager.GetConnection();

        return connection.ExecuteScalarAsync<decimal>(sql, new { AccountId = accountId.ToString() });
    }
}
