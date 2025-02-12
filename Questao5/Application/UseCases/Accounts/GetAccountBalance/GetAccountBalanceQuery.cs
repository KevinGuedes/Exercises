using Questao5.Application.Abstractions.Queries;

namespace Questao5.Application.UseCases.Accounts.GetAccountBalance;

public sealed class GetAccountBalanceQuery : IQuery<GetAccountBalanceResponse>
{
    public Guid AccountId { get; init; }

    /// <summary>
    /// Parameterless constructor to allow fakers to create instances of this class
    /// </summary>
    private GetAccountBalanceQuery()
    {
    }

    public GetAccountBalanceQuery(Guid accountId)
    {
        AccountId = accountId;
    }
}
