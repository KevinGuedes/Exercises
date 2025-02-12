using FluentValidation;
using Questao5.Domain.Interfaces.Repositories;

namespace Questao5.Application.UseCases.Accounts.GetAccountBalance;

public sealed class GetAccountBalanceValidator : AbstractValidator<GetAccountBalanceQuery>
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountBalanceValidator(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(command => command.AccountId)
            .MustAsync(async (accountId, _) =>
            {
                return await _accountRepository.ExistsByIdAsync(accountId);
            })
            .WithErrorCode("INVALID_ACCOUNT")
            .WithMessage("The account does not exist.");

        RuleFor(command => command.AccountId)
            .MustAsync(async (accountId, _) =>
            {
                var account = await _accountRepository.GetByIdAsync(accountId);
                return account!.IsActive;
            })
            .WithErrorCode("INACTIVE_ACCOUNT")
            .WithMessage("The account is inactive.");
    }
}
