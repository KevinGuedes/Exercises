﻿using FluentValidation;
using Questao5.Domain.Interfaces;

namespace Questao5.Application.UseCases.Transfers.RegisterTransfer;

public class RegisterTransferValidator : AbstractValidator<RegisterTransferCommand>
{
    private readonly IAccountRepository _accountRepository;

    public RegisterTransferValidator(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(command => command.Type)
            .Matches("C|D")
            .WithErrorCode("INVALID_TYPE")
            .WithMessage("The type must be 'C' or 'D'.");

        RuleFor(command => command.Value)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode("INVALID_VALUE")
            .WithMessage("The value must be greater than or equal to zero.");

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
