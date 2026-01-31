using FluentValidation;

public class UpdateContribuicaoPresenteValidation : AbstractValidator<UpdateContribuicaoPresenteDto>
{
    public UpdateContribuicaoPresenteValidation()
    {
        RuleFor(c => c.Valor)
            .GreaterThan(0).WithMessage("O valor da contribuição deve ser maior que 0.");

        RuleFor(c => c.Justificativa)
            .MaximumLength(500).WithMessage("A justificativa deve ter até 500 caracteres.");

        RuleFor(c => c.Status)
            .NotNull().WithMessage("O status da contribuição é obrigatório.");

        RuleFor(c => c.Status.Id)
            .GreaterThan(0).WithMessage("O ID do status da contribuição deve ser maior que 0.");
    }
}
