using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina.Application.DTO.CatalogoEstoque;
using Oficina.Application.UseCases.CatalogoEstoque;

namespace Oficina.Api.Controllers;

[ApiController]
[Route("api/servicos")]
[Authorize]
public class ServicosController : ControllerBase
{
    private readonly CadastrarServicoUseCase _cadastrar;

    public ServicosController(CadastrarServicoUseCase cadastrar)
    {
        _cadastrar = cadastrar;
    }

    [HttpPost]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarServicoRequest req, CancellationToken ct)
    {
        var pecas = req.Pecas?.Select(x => (x.Id, x.Quantidade));
        var insumos = req.Insumos?.Select(x => (x.Id, x.Quantidade));

        var id = await _cadastrar.Executar(req.MaoDeObra, pecas, insumos, ct);
        return CreatedAtAction(nameof(Cadastrar), new { id }, new { id });
    }
}
