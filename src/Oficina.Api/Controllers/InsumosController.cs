using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Routing;
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
    private readonly ObterInsumoUseCase _obter;

    public InsumosController(
        CadastrarInsumoUseCase cadastrar,
        ObterInsumoUseCase obter)
    {
        _cadastrar = cadastrar;
        _obter = obter;
    }

    [HttpPost]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarInsumoRequest req, CancellationToken ct)
    {
        var id = await _cadastrar.Executar(req.PrecoUnitario, req.Descricao, ct);
        return CreatedAtAction(nameof(Cadastrar), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken ct)
    {
        var v = await _obter.Executar(id, ct);
        return Ok(new { v.Id, v.Descricao, v.PrecoUnitario });
    }
}
