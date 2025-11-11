using Eventhub.Domain.Entities;
using FluentValidation;

namespace Eventhub.Application.Validations;

public class NotificacaoValidation : AbstractValidator<Notificacao>
{
    public NotificacaoValidation()
    {
        RuleFor(n => n.IdEvento)
            .GreaterThan(0).WithMessage("O evento é obrigatório.");

        RuleFor(n => n.IdUsuarioOrigem)
            .GreaterThan(0).WithMessage("O usuário de origem é obrigatório.");

        RuleFor(n => n.IdUsuarioDestino)
            .GreaterThan(0).WithMessage("O usuário de destino é obrigatório.");

        RuleFor(n => n.Titulo)
            .NotEmpty().WithMessage("O título é obrigatório.")
            .MaximumLength(200).WithMessage("O título deve ter até 200 caracteres.");

        RuleFor(n => n.Descricao)
            .NotEmpty().WithMessage("A descrição é obrigatória.")
            .MaximumLength(1000).WithMessage("A descrição deve ter até 1000 caracteres.");

        RuleFor(n => n.Status)
            .NotEmpty().WithMessage("O status é obrigatório.")
            .MaximumLength(50).WithMessage("O status deve ter até 50 caracteres.");

        RuleFor(n => n.Prioridade)
            .InclusiveBetween(1, 5).WithMessage("A prioridade deve estar entre 1 e 5.");
    }
}
