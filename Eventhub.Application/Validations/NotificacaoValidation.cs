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
            .MaximumLength(100).WithMessage("O título deve ter até 100 caracteres.");

        RuleFor(n => n.Descricao)
            .NotEmpty().WithMessage("A descrição é obrigatória.")
            .MaximumLength(500).WithMessage("A descrição deve ter até 500 caracteres.");

        RuleFor(n => n.Status)
            .IsInEnum().WithMessage("O status deve ser um valor válido (Enviada, Lida).");

        RuleFor(n => n.Prioridade)
            .IsInEnum().WithMessage("A prioridade deve ser um valor válido (Baixa, Média, Alta ou Urgente).");
    }
}
