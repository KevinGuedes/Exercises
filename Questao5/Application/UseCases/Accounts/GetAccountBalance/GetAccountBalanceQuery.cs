using Questao5.Application.Abstractions.Queries;

namespace Questao5.Application.UseCases.Accounts.GetAccountBalance;

public sealed record GetAccountBalanceQuery(Guid AccountId) : IQuery<GetAccountBalanceResponse>
{
}
