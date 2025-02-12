using FluentResults;
using Questao5.Application.Abstractions.Commands;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces;

namespace Questao5.Application.UseCases.Transfers.RegisterTransfer;

public sealed class RegisterTransferHandler(ITransferRepository transferRepository) 
    : ICommandHandler<RegisterTransferCommand, RegisterTransferResponse>
{
    private readonly ITransferRepository _transferRepository = transferRepository;

    public async Task<Result<RegisterTransferResponse>> Handle(RegisterTransferCommand command, CancellationToken cancellationToken)
    {
        var transfer = new Transfer(
            command.AccountId, 
            command.Date,
            command.Type, 
            command.Value);

        await _transferRepository.InsertAsync(transfer);

        var response = new RegisterTransferResponse(
            transfer.Id,
            transfer.AccountId,
            transfer.Value,
            transfer.Type);

        return Result.Ok(response);
    }
}
