using Eventhub.Domain.Entities;
using FluentValidation;

namespace Eventhub.Application.Validations
{
    public class EnderecoEventoValidation : AbstractValidator<EnderecoEvento>
    {
        public EnderecoEventoValidation()
        {
            RuleFor(e => e.Numero)
                .NotEmpty().WithMessage("O número é obrigatório.")
                .MaximumLength(20).WithMessage("O número deve ter no máximo 20 caracteres.");

            RuleFor(e => e.Logradouro)
                .NotEmpty().WithMessage("O logradouro é obrigatório.")
                .MaximumLength(200).WithMessage("O logradouro deve ter no máximo 200 caracteres.");

            RuleFor(e => e.Cep)
                .NotEmpty().WithMessage("O CEP é obrigatório.")
                .MaximumLength(10).WithMessage("O CEP deve ter no máximo 10 caracteres.");

            RuleFor(e => e.Cidade)
                .NotEmpty().WithMessage("A cidade é obrigatória.")
                .MaximumLength(100).WithMessage("A cidade deve ter no máximo 100 caracteres.");

            RuleFor(e => e.PontoReferencia)
                .MaximumLength(200).WithMessage("O ponto de referência deve ter no máximo 200 caracteres.");
        }
    }
}
