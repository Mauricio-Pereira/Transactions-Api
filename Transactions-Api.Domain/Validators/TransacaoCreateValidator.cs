using Transactions_Api.Domain.Models;
using FluentValidation;

namespace Transactions_Api.Domain.Validators;

public class TransacaoCreateValidator : AbstractValidator<Transacao>
{
    public TransacaoCreateValidator()
    {
            RuleFor(t => t.Valor)
                .GreaterThan(0).WithMessage("O valor da transação deve ser positivo.");
    }
}