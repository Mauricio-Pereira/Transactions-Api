using FluentValidation;
using Transactions_Api.Application.Commands;

namespace Transactions_Api.Application.Validators;

public class DeleteTransactionCommandValidator : AbstractValidator<DeleteTransactionCommand>
{
    public DeleteTransactionCommandValidator()
    {
        RuleFor(c => c.Txid)
            .NotEmpty().WithMessage("O Txid não pode ser vazio.")
            .NotNull().WithMessage("O Txid é obrigatório.");
    }
}