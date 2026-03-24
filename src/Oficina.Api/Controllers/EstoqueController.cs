using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina.Application.DTO.CatalogoEstoque;
using Oficina.Application.UseCases.CatalogoEstoque;

namespace Oficina.Api.Controllers;

[ApiController]
[Route("api/estoque")]
[Authorize]
public class EstoqueController : ControllerBase
{
    private readonly ObterEstoquePecaUseCase _obterPeca;
    private readonly ObterEstoqueInsumoUseCase _obterInsumo;
    private readonly AjustarEstoquePecaUseCase _ajustarPeca;
    private readonly AjustarEstoqueInsumoUseCase _ajustarInsumo;

    public EstoqueController(
        ObterEstoquePecaUseCase obterPeca,
        ObterEstoqueInsumoUseCase obterInsumo,
        AjustarEstoquePecaUseCase ajustarPeca,
        AjustarEstoqueInsumoUseCase ajustarInsumo)
    {
        _obterPeca = obterPeca;
        _obterInsumo = obterInsumo;
        _ajustarPeca = ajustarPeca;
        _ajustarInsumo = ajustarInsumo;
    }

    [HttpGet("pecas/{pecaId:guid}")]
    public async Task<IActionResult> ObterPeca(Guid pecaId, CancellationToken ct)
    {
        var item = await _obterPeca.Executar(pecaId, ct);
        return Ok(item);
    }

    [HttpPost("pecas/{pecaId:guid}/ajustar")]
    public async Task<IActionResult> AjustarPeca(Guid pecaId, [FromBody] AjustarEstoqueRequest req, CancellationToken ct)
    {
        await _ajustarPeca.Executar(pecaId, req.Quantidade, ct);
        return NoContent();
    }

    [HttpGet("insumos/{insumoId:guid}")]
    public async Task<IActionResult> ObterInsumo(Guid insumoId, CancellationToken ct)
    {
        var item = await _obterInsumo.Executar(insumoId, ct);
        return Ok(item);
    }

    [HttpPost("insumos/{insumoId:guid}/ajustar")]
    public async Task<IActionResult> AjustarInsumo(Guid insumoId, [FromBody] AjustarEstoqueRequest req, CancellationToken ct)
    {
        await _ajustarInsumo.Executar(insumoId, req.Quantidade, ct);
        return NoContent();
    }
}
