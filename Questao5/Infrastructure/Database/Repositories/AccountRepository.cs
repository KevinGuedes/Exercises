using Dapper;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces.Repositories;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.Infrastructure.Database.Repositories;

public sealed class AccountRepository(ISqlConnectionManager sqlConnectionManager) : IAccountRepository
{
    private readonly ISqlConnectionManager _sqlConnectionManager = sqlConnectionManager;

    public Task<bool> ExistsByIdAsync(Guid accountId)
    {
        const string sql = @"
            SELECT 
                COUNT(1) 
            FROM 
                contacorrente
            WHERE 
                idcontacorrente = @AccountId
        ";
        using var connection = _sqlConnectionManager.GetConnection();
        return connection.ExecuteScalarAsync<bool>(sql, new { AccountId = accountId });
    }

    public Task<Account?> GetByIdAsync(Guid accountId)
    {
        const string sql = @"
            SELECT 
                idcontacorrente AS Id, 
                nome AS HolderName, 
                ativo AS IsActive,
                numero AS Number
            FROM 
                contacorrente
            WHERE 
                idcontacorrente = @AccountId
        ";

        using var connection = _sqlConnectionManager.GetConnection();
        return connection.QueryFirstOrDefaultAsync<Account>(sql, new { AccountId = accountId });
    }

    public Task<decimal> GetBalanceAsync(Guid accountId)
    {
        const string sql = @"
            SELECT 
                COALESCE(SUM(CASE WHEN tipomovimento = 'C' THEN valor ELSE 0 END), 0) -
                COALESCE(SUM(CASE WHEN tipomovimento = 'D' THEN valor ELSE 0 END), 0) 
            FROM 
                movimento
            WHERE 
                idcontacorrente = @AccountId
        ";

        using var connection = _sqlConnectionManager.GetConnection();

        return connection.ExecuteScalarAsync<decimal>(sql, new { AccountId = accountId });
    }
}
