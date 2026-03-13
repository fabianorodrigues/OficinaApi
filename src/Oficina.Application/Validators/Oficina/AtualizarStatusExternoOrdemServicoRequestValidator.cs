using FluentValidation;
using Oficina.Application.DTO.Oficina;

namespace Oficina.Application.Validators.Oficina;

public class AtualizarStatusExternoOrdemServicoRequestValidator : AbstractValidator<AtualizarStatusExternoOrdemServicoRequest>
{
    public AtualizarStatusExternoOrdemServicoRequestValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Origem).NotEmpty();
    }
}
