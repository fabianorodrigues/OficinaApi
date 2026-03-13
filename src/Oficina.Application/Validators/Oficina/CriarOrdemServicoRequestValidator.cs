using FluentValidation;
using Oficina.Application.DTO.Oficina;
using Oficina.Domain.Oficina.Enums;

namespace Oficina.Application.Validators.Oficina;

public class CriarOrdemServicoRequestValidator : AbstractValidator<CriarOrdemServicoRequest>
{
    public CriarOrdemServicoRequestValidator()
    {
        RuleFor(x => x.VeiculoId).NotEmpty();
        RuleFor(x => x.TipoManutencao).IsInEnum();

        When(x => x.TipoManutencao == TipoManutencao.Preventiva, () =>
        {
            RuleFor(x => x.ServicoIds)
                .NotNull()
                .Must(x => x is not null && x.Count > 0)
                .WithMessage("Informe ao menos 1 serviço para OS preventiva.");
        });
    }
}
