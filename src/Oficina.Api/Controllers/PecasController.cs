using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina.Application.DTO.CatalogoEstoque;
using Oficina.Application.UseCases.CatalogoEstoque;

namespace Oficina.Api.Controllers;

[ApiController]
[Route("api/pecas")]
[Authorize]
public class PecasController : ControllerBase
{
    private readonly CadastrarPecaUseCase _cadastrar;
    private readonly ObterPecaUseCase _obter;
    private readonly AtualizarPecaUseCase _atualizar;

    public PecasController(
        CadastrarPecaUseCase cadastrar,
        ObterPecaUseCase obter,
        AtualizarPecaUseCase atualizar)
    {
        _cadastrar = cadastrar;
        _obter = obter;
        _atualizar = atualizar;
    }

    [HttpPost]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarPecaRequest req, CancellationToken ct)
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

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarPecaRequest req, CancellationToken ct)
    {
        await _atualizar.Executar(id, req.PrecoUnitario, req.Descricao, ct);
        return NoContent();
    }
}
