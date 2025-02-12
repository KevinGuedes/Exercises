using FluentResults;
using Questao5.Application.Abstractions.Queries;
using Questao5.Domain.Interfaces.Repositories;

namespace Questao5.Application.UseCases.Accounts.GetAccountBalance;

public sealed class GetAccountBalanceHandler(IAccountRepository accountRepository)
    : IQueryHandler<GetAccountBalanceQuery, GetAccountBalanceResponse>
{
    private readonly IAccountRepository _accountRepository = accountRepository;

    public async Task<Result<GetAccountBalanceResponse>> Handle(GetAccountBalanceQuery request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(request.AccountId);
        var balance = await _accountRepository.GetBalanceAsync(request.AccountId);

        var response = new GetAccountBalanceResponse(
            account!.Number,
            account.HolderName,
            DateTime.UtcNow,
            balance);

        return Result.Ok(response);
    }
}
