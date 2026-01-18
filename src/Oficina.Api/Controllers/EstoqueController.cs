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
    private readonly ObterPecaUseCase _obterPecaCatalogo;
    private readonly ObterInsumoUseCase _obterInsumoCatalogo;

    public EstoqueController(
        ObterEstoquePecaUseCase obterPeca,
        ObterEstoqueInsumoUseCase obterInsumo,
        AjustarEstoquePecaUseCase ajustarPeca,
        AjustarEstoqueInsumoUseCase ajustarInsumo,
        ObterPecaUseCase obterPecaCatalogo,
        ObterInsumoUseCase obterInsumoCatalogo)
    {
        _obterPeca = obterPeca;
        _obterInsumo = obterInsumo;
        _ajustarPeca = ajustarPeca;
        _ajustarInsumo = ajustarInsumo;
        _obterPecaCatalogo = obterPecaCatalogo;
        _obterInsumoCatalogo = obterInsumoCatalogo;
    }

    [HttpGet("pecas/{pecaId:guid}")]
    public async Task<IActionResult> ObterPeca(Guid pecaId, CancellationToken ct)
    {
        var qtd = await _obterPeca.Executar(pecaId, ct);
        var peca = await _obterPecaCatalogo.Executar(pecaId, ct);
        return Ok(new { pecaId, peca.Descricao, quantidade = qtd });
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
        var qtd = await _obterInsumo.Executar(insumoId, ct);
        var insumo = await _obterInsumoCatalogo.Executar(insumoId, ct);
        return Ok(new { insumoId, insumo.Descricao, quantidade = qtd });
    }

    [HttpPost("insumos/{insumoId:guid}/ajustar")]
    public async Task<IActionResult> AjustarInsumo(Guid insumoId, [FromBody] AjustarEstoqueRequest req, CancellationToken ct)
    {
        await _ajustarInsumo.Executar(insumoId, req.Quantidade, ct);
        return NoContent();
    }
}
