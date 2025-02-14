using Questao5.Application.Abstractions.Commands;

namespace Questao5.Application.UseCases.Transfers.RegisterTransfer;

public sealed class RegisterTransferCommand : ICommand<RegisterTransferResponse>
{
    public Guid Key { get; init; }
    public Guid AccountId { get; init; }
    public decimal Value { get; init; }
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// Parameterless constructor to allow fakers to create instances of this class
    /// </summary>
    private RegisterTransferCommand()
    {
    }

    public RegisterTransferCommand(Guid key, Guid accountId, decimal value, string type)
    {
        Key = key;
        AccountId = accountId;
        Value = value;
        Type = type;
    }
}