using FluentValidation;
using Oficina.Application.DTO.CatalogoEstoque;

namespace Oficina.Application.Validators.CatalogoEstoque;

public class CadastrarInsumoRequestValidator : AbstractValidator<CadastrarInsumoRequest>
{
    public CadastrarInsumoRequestValidator()
    {
        RuleFor(x => x.PrecoUnitario).GreaterThanOrEqualTo(0);
    }
}
