using Transactions_Api.Domain.Models;
using FluentValidation;

namespace Transactions_Api.Domain.Validators;

public class TransacaoUpdateDTOValidator : AbstractValidator<Transacao>
{
    public TransacaoUpdateDTOValidator()
    {
        // Txid Validation
        RuleFor(t => t.Txid)
            .NotEmpty().WithMessage("O Txid é obrigatório.")
            .Length(26, 35).WithMessage("O Txid deve ter entre 26 e 35 caracteres.")
            .Matches("^[a-zA-Z0-9]+$").WithMessage("O Txid deve conter apenas caracteres alfanuméricos.");

        // Payer Data Validation
        RuleFor(t => t.PagadorNome)
            .NotEmpty().WithMessage("O nome do pagador é obrigatório.")
            .MaximumLength(100).WithMessage("O nome do pagador deve ter no máximo 100 caracteres.");

        RuleFor(t => t.PagadorCpf)
            .NotEmpty().WithMessage("O CPF do pagador é obrigatório.")
            .Length(11).WithMessage("O CPF do pagador deve ter 11 dígitos.")
            .Must(IsCpfValido).WithMessage("CPF do pagador inválido.");

        RuleFor(t => t.PagadorBanco)
            .NotEmpty().WithMessage("O banco do pagador é obrigatório.")
            .MaximumLength(8).WithMessage("O banco do pagador deve ter no máximo 8 caracteres.")
            .Matches("^[0-9]+$").WithMessage("O banco do pagador deve conter apenas números.");

        RuleFor(t => t.PagadorAgencia)
            .NotEmpty().WithMessage("A agência do pagador é obrigatória.")
            .MaximumLength(6).WithMessage("A agência do pagador deve ter no máximo 6 caracteres.")
            .Matches("^[0-9]+$").WithMessage("A agência do pagador deve conter apenas números.");

        RuleFor(t => t.PagadorConta)
            .NotEmpty().WithMessage("A conta do pagador é obrigatória.")
            .MaximumLength(10).WithMessage("A conta do pagador deve ter no máximo 10 caracteres.")
            .Matches("^[0-9]+$").WithMessage("A conta do pagador deve conter apenas números.");

        // Receiver Data Validation
        RuleFor(t => t.RecebedorNome)
            .NotEmpty().WithMessage("O nome do recebedor é obrigatório.")
            .MaximumLength(100).WithMessage("O nome do recebedor deve ter no máximo 100 caracteres.");

        RuleFor(t => t.RecebedorCpf)
            .NotEmpty().WithMessage("O CPF do recebedor é obrigatório.")
            .Matches("^[0-9]+$").WithMessage("O CPF do recebedor deve conter apenas números.")
            .Must(IsCpfValido).WithMessage("CPF do recebedor inválido.");

        RuleFor(t => t.RecebedorBanco)
            .NotEmpty().WithMessage("O banco do recebedor é obrigatório.")
            .MaximumLength(8).WithMessage("O banco do recebedor deve ter no máximo 8 caracteres.")
            .Matches("^[0-9]+$").WithMessage("O banco do recebedor deve conter apenas números.");

        RuleFor(t => t.RecebedorAgencia)
            .NotEmpty().WithMessage("A agência do recebedor é obrigatória.")
            .MaximumLength(6).WithMessage("A agência do recebedor deve ter no máximo 6 caracteres.")
            .Matches("^[0-9]+$").WithMessage("A agência do recebedor deve conter apenas números.");

        RuleFor(t => t.RecebedorConta)
            .NotEmpty().WithMessage("A conta do recebedor é obrigatória.")
            .MaximumLength(10).WithMessage("A conta do recebedor deve ter no máximo 10 caracteres.")
            .Matches("^[0-9]+$").WithMessage("A conta do recebedor deve conter apenas números.");
    }

    private bool IsCpfValido(string cpf)
    { 
        // Validation for CPF based on https://www.geradorcpf.com/algoritmo_do_cpf.htm
        // To implement, uncomment the code below and comment the return true statement
        /*
    // Implementation of the CPF validation algorithm
    if (string.IsNullOrWhiteSpace(cpf) || cpf.Length != 11 || !cpf.All(char.IsDigit))
        return false;

    // Elimina CPFs inválidos conhecidos
    if (cpf.All(c => c == cpf[0]))
        return false;

    int[] multiplicador1 = new int[9]{10,9,8,7,6,5,4,3,2};
    int[] multiplicador2 = new int[10]{11,10,9,8,7,6,5,4,3,2};

    string tempCpf = cpf.Substring(0, 9);
    int soma = 0;

    for (int i = 0; i < 9; i++)
        soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

    int resto = soma % 11;
    resto = resto < 2 ? 0 : 11 - resto;

    string digito = resto.ToString();
    tempCpf += digito;

    soma = 0;
    for (int i = 0; i < 10; i++)
        soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

    resto = soma % 11;
    resto = resto < 2 ? 0 : 11 - resto;

    digito += resto.ToString();

    return cpf.EndsWith(digito);*/
        return true;
    }
}