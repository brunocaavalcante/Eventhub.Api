using Eventhub.Domain.Entities;
using FluentValidation;

namespace Eventhub.Application.Validations;

public class ProgramacaoEventoValidation : AbstractValidator<ProgramacaoEvento>
{
    public ProgramacaoEventoValidation()
    {
        RuleFor(p => p.IdEvento)
            .GreaterThan(0).WithMessage("O evento é obrigatório.");

        RuleFor(p => p.Titulo)
            .NotEmpty().WithMessage("O título é obrigatório.")
            .MaximumLength(200).WithMessage("O título deve ter até 200 caracteres.");

        RuleFor(p => p.Descricao)
            .MaximumLength(1000).WithMessage("A descrição deve ter até 1000 caracteres.");

        RuleFor(p => p.Data)
            .NotEmpty().WithMessage("A data é obrigatória.");

        RuleFor(p => p.Duracao)
            .NotEmpty().WithMessage("A duração é obrigatória.");

        RuleFor(p => p.Local)
            .MaximumLength(200).WithMessage("O local deve ter até 200 caracteres.");

        RuleFor(p => p.IdFoto)
            .GreaterThan(0).WithMessage("A foto é obrigatória.");

        RuleFor(p => p.Responsavel)
            .MaximumLength(200).WithMessage("O responsável deve ter até 200 caracteres.");

        RuleFor(p => p.IdStatus)
            .GreaterThan(0).WithMessage("O status é obrigatório.");
    }
}
