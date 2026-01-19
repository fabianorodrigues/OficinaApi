using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina.Application.Abstractions.Repositorios;
using Oficina.Application.DTO.Oficina;
using Oficina.Application.UseCases.Oficina;
using Oficina.Domain.Oficina.Enums;

namespace Oficina.Api.Controllers;

[ApiController]
[Route("api/ordens-servico")]
[Authorize]
public class OrdensServicoController : ControllerBase
{
    private readonly CriarOsPreventivaUseCase _criarPreventiva;
    private readonly CriarOsCorretivaUseCase _criarCorretiva;
    private readonly RegistrarDiagnosticoUseCase _registrarDiagnostico;
    private readonly ObterOrdemServicoUseCase _obter;
    private readonly ListarOrdensServicoUseCase _listar;
    private readonly FinalizarOrdemServicoUseCase _finalizar;
    private readonly EntregarOrdemServicoUseCase _entregar;
    private readonly ICatalogoEstoqueRepository _catalogo;

    public OrdensServicoController(
        CriarOsPreventivaUseCase criarPreventiva,
        CriarOsCorretivaUseCase criarCorretiva,
        RegistrarDiagnosticoUseCase registrarDiagnostico,
        ObterOrdemServicoUseCase obter,
        ListarOrdensServicoUseCase listar,
        FinalizarOrdemServicoUseCase finalizar,
        EntregarOrdemServicoUseCase entregar,
        ICatalogoEstoqueRepository catalogo)
    {
        _criarPreventiva = criarPreventiva;
        _criarCorretiva = criarCorretiva;
        _registrarDiagnostico = registrarDiagnostico;
        _obter = obter;
        _listar = listar;
        _finalizar = finalizar;
        _entregar = entregar;
        _catalogo = catalogo;
    }

    [HttpPost("preventiva")]
    public async Task<IActionResult> CriarPreventiva([FromBody] CriarOsPreventivaRequest req, CancellationToken ct)
    {
        var (osId, orcamentoId) = await _criarPreventiva.Executar(req.VeiculoId, req.ServicoIds, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = osId }, new { id = osId, orcamentoId });
    }

    [HttpPost("corretiva")]
    public async Task<IActionResult> CriarCorretiva([FromBody] CriarOsCorretivaRequest req, CancellationToken ct)
    {
        var osId = await _criarCorretiva.Executar(req.VeiculoId, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = osId }, new { id = osId });
    }

    [HttpPost("{id:guid}/diagnosticos")]
    public async Task<IActionResult> RegistrarDiagnostico(Guid id, [FromBody] RegistrarDiagnosticoRequest req, CancellationToken ct)
    {
        var orcamentoId = await _registrarDiagnostico.Executar(id, req.Descricao, req.ServicoIds, ct);
        return Ok(new { orcamentoId });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken ct)
    {
        var (os, orcamento) = await _obter.Executar(id, ct);

        List<object>? itensMaterial = null;
        if (orcamento is not null)
        {
            itensMaterial = new List<object>();
            foreach (var x in orcamento.ItensMaterial)
            {
                string? descricao = null;
                if (x.Tipo == TipoMaterial.Peca)
                    descricao = (await _catalogo.ObterPeca(x.MaterialId, ct))?.Descricao;
                else
                    descricao = (await _catalogo.ObterInsumo(x.MaterialId, ct))?.Descricao;

                itensMaterial.Add(new { tipo = x.Tipo.ToString(), x.MaterialId, x.Quantidade, x.ValorUnitario, descricao });
            }
        }

        return Ok(new
        {
            os.Id,
            os.VeiculoId,
            tipoManutencao = os.TipoManutencao.ToString(),
            status = os.Status.ToString(),
            os.DataCriacao,
            os.DataInicioExecucao,
            os.DataFimExecucao,
            diagnostico = os.Diagnostico is null ? null : new { os.Diagnostico.Descricao, os.Diagnostico.DataRegistro },
            orcamento = orcamento is null ? null : new
            {
                orcamento.Id,
                status = orcamento.Status.ToString(),
                orcamento.ValorTotal,
                itensServico = orcamento.ItensServico.Select(x => new { x.ServicoId, x.ValorMaoDeObra }),
                itensMaterial
            }
        });
    }

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        var lista = await _listar.Executar(ct);
        return Ok(lista.Select(os => new
        {
            os.Id,
            os.VeiculoId,
            tipoManutencao = os.TipoManutencao.ToString(),
            status = os.Status.ToString(),
            os.DataCriacao
        }));
    }

    [HttpPost("{id:guid}/finalizar")]
    public async Task<IActionResult> Finalizar(Guid id, CancellationToken ct)
    {
        await _finalizar.Executar(id, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/entregar")]
    public async Task<IActionResult> Entregar(Guid id, CancellationToken ct)
    {
        await _entregar.Executar(id, ct);
        return NoContent();
    }
}
