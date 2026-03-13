using FluentValidation;
using Oficina.Application.DTO.Oficina;
using Oficina.Domain.Oficina.Enums;

namespace Oficina.Application.Validators.Oficina;

public class NotificarOrcamentoRequestValidator : AbstractValidator<NotificarOrcamentoRequest>
{
    public NotificarOrcamentoRequestValidator()
    {
        RuleFor(x => x.OrcamentoId).NotEmpty();
        RuleFor(x => x.Status)
            .Must(s => s == StatusOrcamento.Aprovado || s == StatusOrcamento.Recusado)
            .WithMessage("Status permitido: Aprovado ou Recusado.");
    }
}
