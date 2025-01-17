using FluentValidation;
using Transactions_Api.Application.Commands;

namespace Transactions_Api.Application.Validators;

public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(c => c.Valor)
            .GreaterThan(0).WithMessage("O valor deve ser maior que zero.");
    }
}