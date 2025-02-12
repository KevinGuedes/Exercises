using FluentValidation;

namespace Questao5.Application.UseCases.Transfers.RegisterTransfer;

public class RegisterTransferValidator : AbstractValidator<RegisterTransferCommand>
{
    public RegisterTransferValidator()
    {
        RuleFor(command => command.Value)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode("INVALID_VALUE")
            .WithMessage("The value must be greater than or equal to zero.");
    }
}
