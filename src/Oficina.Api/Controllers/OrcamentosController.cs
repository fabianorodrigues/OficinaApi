using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina.Application.Abstractions.Repositorios;
using Oficina.Application.UseCases.Oficina;
using Oficina.Domain.Oficina.Enums;

namespace Oficina.Api.Controllers;

[ApiController]
[Route("api/orcamentos")]
[Authorize]
public class OrcamentosController : ControllerBase
{
    private readonly ObterOrcamentoUseCase _obter;
    private readonly AprovarOrcamentoUseCase _aprovar;
    private readonly RecusarOrcamentoUseCase _recusar;
    private readonly ICatalogoEstoqueRepository _catalogo;

    public OrcamentosController(
        ObterOrcamentoUseCase obter,
        AprovarOrcamentoUseCase aprovar,
        RecusarOrcamentoUseCase recusar,
        ICatalogoEstoqueRepository catalogo)
    {
        _obter = obter;
        _aprovar = aprovar;
        _recusar = recusar;
        _catalogo = catalogo;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken ct)
    {
        var o = await _obter.Executar(id, ct);

        var itensMaterial = new List<object>();
        foreach (var x in o.ItensMaterial)
        {
            string? descricao = null;
            if (x.Tipo == TipoMaterial.Peca)
                descricao = (await _catalogo.ObterPeca(x.MaterialId, ct))?.Descricao;
            else
                descricao = (await _catalogo.ObterInsumo(x.MaterialId, ct))?.Descricao;

            itensMaterial.Add(new { tipo = x.Tipo.ToString(), x.MaterialId, x.Quantidade, x.ValorUnitario, descricao });
        }
        return Ok(new
        {
            o.Id,
            o.OrdemServicoId,
            status = o.Status.ToString(),
            o.ValorTotal,
            o.DataCriacao,
            itensServico = o.ItensServico.Select(x => new { x.ServicoId, x.ValorMaoDeObra }),
            itensMaterial
        });
    }

    [HttpPost("{id:guid}/aprovar")]
    public async Task<IActionResult> Aprovar(Guid id, CancellationToken ct)
    {
        await _aprovar.Executar(id, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/recusar")]
    public async Task<IActionResult> Recusar(Guid id, CancellationToken ct)
    {
        await _recusar.Executar(id, ct);
        return NoContent();
    }
}
