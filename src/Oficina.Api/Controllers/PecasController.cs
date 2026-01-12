using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using Oficina.Application.DTO.CatalogoEstoque;
using Oficina.Application.UseCases.Cadastro;
using Oficina.Application.UseCases.CatalogoEstoque;

namespace Oficina.Api.Controllers;

[ApiController]
[Route("api/pecas")]
[Authorize]
public class PecasController : ControllerBase
{
    private readonly CadastrarPecaUseCase _cadastrar;
    private readonly ObterPecaUseCase _obter;

    public PecasController(
        CadastrarPecaUseCase cadastrar,
        ObterPecaUseCase obter)
    {
        _cadastrar = cadastrar;
        _obter = obter;
    }

    [HttpPost]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarPecaRequest req, CancellationToken ct)
    {
        var id = await _cadastrar.Executar(req.PrecoUnitario, ct);
        return CreatedAtAction(nameof(Cadastrar), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken ct)
    {
        var v = await _obter.Executar(id, ct);
        return Ok(new { v.Id, v.PrecoUnitario });
    }
}
