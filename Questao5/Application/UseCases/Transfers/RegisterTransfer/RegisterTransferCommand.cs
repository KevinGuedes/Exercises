using Questao5.Application.Abstractions.Commands;

namespace Questao5.Application.UseCases.Transfers.RegisterTransfer;

public sealed record RegisterTransferCommand(
    Guid Key, 
    Guid AccountId,
    DateOnly Date,
    decimal Value, 
    string Type)
    : ICommand<RegisterTransferResponse>
{
}
