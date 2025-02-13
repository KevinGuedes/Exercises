using Dapper;
using Microsoft.Data.Sqlite;
using Questao5.Infrastructure.Sqlite;
using Questao5.TestCommon.TestData;

namespace Questao5.IntegrationTests.Common;

public sealed class TestDatabaseBootstrap(DatabaseConfig dbConfig) : IDatabaseBootstrap
{
    private readonly DatabaseConfig _dbConfig = dbConfig;

    public void Setup()
    {
        using var connection = new SqliteConnection(_dbConfig.ConnectionString);

        var table = connection.Query<string>("SELECT name FROM sqlite_master WHERE type='table' AND (name = 'contacorrente' or name = 'movimento' or name = 'idempotencia');");
        var tableName = table.FirstOrDefault();
        if (!string.IsNullOrEmpty(tableName) && (tableName == "contacorrente" || tableName == "movimento" || tableName == "idempotencia"))
            return;

        connection.Execute("CREATE TABLE contacorrente ( " +
                           "idcontacorrente TEXT(37) PRIMARY KEY," +
                           "numero INTEGER(10) NOT NULL UNIQUE," +
                           "nome TEXT(100) NOT NULL," +
                           "ativo INTEGER(1) NOT NULL default 0," +
                           "CHECK(ativo in (0, 1)) " +
                           ");");

        connection.Execute("CREATE TABLE movimento ( " +
            "idmovimento TEXT(37) PRIMARY KEY," +
            "idcontacorrente TEXT(37) NOT NULL," +
            "datamovimento TEXT(25) NOT NULL," +
            "tipomovimento TEXT(1) NOT NULL," +
            "valor REAL NOT NULL," +
            "CHECK(tipomovimento in ('C', 'D')), " +
            "FOREIGN KEY(idcontacorrente) REFERENCES contacorrente(idcontacorrente) " +
            ");");

        connection.Execute("CREATE TABLE idempotencia (" +
                           "chave_idempotencia TEXT(37) PRIMARY KEY," +
                           "requisicao TEXT(1000)," +
                           "resultado TEXT(1000));");

        var validAccount = AccountTestData.CreateActiveAccount();
        connection.Execute("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES(@Id, @Number, @HolderName, @IsActive);", validAccount);

        var invalidAccount = AccountTestData.CreateInactiveAccount();
        connection.Execute("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES(@Id, @Number, @HolderName, @IsActive);", invalidAccount);

        var transfer = TransferTestData.CreateCreditTransferForAccount(validAccount.Id);
        connection.Execute("INSERT INTO movimento(idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) VALUES(@Id, @AccountId, @Date, @Type, @Value);", new
        {
            transfer.Id,
            transfer.AccountId,
            transfer.Type,
            transfer.Value,
            Date = transfer.Date.ToString("dd/MM/yyyy")
        });
    }
}
