using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina.Application.DTO.CatalogoEstoque;
using Oficina.Application.UseCases.CatalogoEstoque;

namespace Oficina.Api.Controllers;

[ApiController]
[Route("api/insumos")]
[Authorize]
public class InsumosController : ControllerBase
{
    private readonly CadastrarInsumoUseCase _cadastrar;

    public InsumosController(CadastrarInsumoUseCase cadastrar) => _cadastrar = cadastrar;

    [HttpPost]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarInsumoRequest req, CancellationToken ct)
    {
        var id = await _cadastrar.Executar(req.PrecoUnitario, ct);
        return CreatedAtAction(nameof(Cadastrar), new { id }, new { id });
    }
}
