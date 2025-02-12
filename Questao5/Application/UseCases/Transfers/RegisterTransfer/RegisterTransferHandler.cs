using FluentResults;
using Questao5.Application.Abstractions.Commands;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces.Repositories;
using Questao5.Domain.Interfaces.Services;

namespace Questao5.Application.UseCases.Transfers.RegisterTransfer;

public sealed class RegisterTransferHandler(
    ITransferRepository transferRepository,
    IIdempotencyRepository idempotencyRepository,
    ISerializerService serializerService)
    : ICommandHandler<RegisterTransferCommand, RegisterTransferResponse>
{
    private readonly ITransferRepository _transferRepository = transferRepository;
    private readonly IIdempotencyRepository _idempotencyRepository = idempotencyRepository;
    private readonly ISerializerService _serializerService = serializerService;

    public async Task<Result<RegisterTransferResponse>> Handle(RegisterTransferCommand command, CancellationToken cancellationToken)
    {
        var idempotency = await _idempotencyRepository.GetByKeyAsync(command.Key);

        if (idempotency is not null)
        {
            var previousResult = _serializerService.Deserialize<RegisterTransferResponse>(idempotency.Result);
            return Result.Ok(previousResult);
        }

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

        var newIdempotency = new Idempotency(
            command.Key,
            _serializerService.Serialize(command),
            _serializerService.Serialize(response));

        await _idempotencyRepository.InsertAsync(newIdempotency);

        return Result.Ok(response);
    }
}
