using Dapper;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces.Repositories;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.Infrastructure.Database.Repositories;

public sealed class TransferRepository(ISqlConnectionManager sqlConnectionManager) : ITransferRepository
{
    private readonly ISqlConnectionManager _sqlConnectionManager = sqlConnectionManager;

    public Task InsertAsync(Transfer transfer)
    {
        const string sql = @"
            INSERT INTO movimento 
                (idmovimento, idcontacorrente, tipomovimento, valor, datamovimento)
            VALUES 
                (@Id, @AccountId, @Type, @Value, @Date)
        ";

        using var connection = _sqlConnectionManager.GetConnection();
        return connection.ExecuteAsync(sql, new
        {
            transfer.Id,
            transfer.AccountId,
            transfer.Type,
            transfer.Value,
            Date = transfer.Date.ToString("dd/MM/yyyy")
        });
    }
}
