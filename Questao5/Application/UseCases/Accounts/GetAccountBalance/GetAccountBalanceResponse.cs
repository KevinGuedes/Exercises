namespace Questao5.Application.UseCases.Accounts.GetAccountBalance;

public sealed record GetAccountBalanceResponse(
    long AccountNumber,
    string AccountHolderName,
    DateTime BalanceCalculatedOnUtc,
    decimal Balance)
{
}

