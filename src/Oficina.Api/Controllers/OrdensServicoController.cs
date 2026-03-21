using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficina.Application.DTO.Oficina;
using Oficina.Application.UseCases.Oficina;

namespace Oficina.Api.Controllers;

[ApiController]
[Route("api/ordens-servico")]
[Authorize]
public class OrdensServicoController : ControllerBase
{
    private readonly AbrirOrdemServicoUseCase _abrirOrdemServico;
    private readonly CriarOsPreventivaUseCase _criarPreventiva;
    private readonly CriarOsCorretivaUseCase _criarCorretiva;
    private readonly RegistrarDiagnosticoUseCase _registrarDiagnostico;
    private readonly ClassificarOrdemServicoUseCase _classificar;
    private readonly ObterStatusOrdemServicoUseCase _obterStatus;
    private readonly ObterOrdemServicoDetalhadaUseCase _obter;
    private readonly ListarOrdensServicoUseCase _listar;
    private readonly FinalizarOrdemServicoUseCase _finalizar;
    private readonly EntregarOrdemServicoUseCase _entregar;

    public OrdensServicoController(
        AbrirOrdemServicoUseCase abrirOrdemServico,
        CriarOsPreventivaUseCase criarPreventiva,
        CriarOsCorretivaUseCase criarCorretiva,
        RegistrarDiagnosticoUseCase registrarDiagnostico,
        ClassificarOrdemServicoUseCase classificar,
        ObterStatusOrdemServicoUseCase obterStatus,
        ObterOrdemServicoDetalhadaUseCase obter,
        ListarOrdensServicoUseCase listar,
        FinalizarOrdemServicoUseCase finalizar,
        EntregarOrdemServicoUseCase entregar)
    {
        _abrirOrdemServico = abrirOrdemServico;
        _criarPreventiva = criarPreventiva;
        _criarCorretiva = criarCorretiva;
        _registrarDiagnostico = registrarDiagnostico;
        _classificar = classificar;
        _obterStatus = obterStatus;
        _obter = obter;
        _listar = listar;
        _finalizar = finalizar;
        _entregar = entregar;
    }


    [HttpPost]
    public async Task<IActionResult> Abrir([FromBody] AbrirOrdemServicoRequest req, CancellationToken ct)
    {
        var resultado = await _abrirOrdemServico.Executar(req, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Id }, resultado);
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


    [HttpPost("{id:guid}/classificar")]
    public async Task<IActionResult> Classificar(Guid id, [FromBody] ClassificarOrdemServicoRequest req, CancellationToken ct)
    {
        await _classificar.Executar(id, req.TipoManutencao, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/diagnosticos")]
    public async Task<IActionResult> RegistrarDiagnostico(Guid id, [FromBody] RegistrarDiagnosticoRequest req, CancellationToken ct)
    {
        var orcamentoId = await _registrarDiagnostico.Executar(id, req.Descricao, req.ServicoIds, ct);
        return Ok(new { orcamentoId });
    }


    [HttpGet("{id:guid}/status")]
    public async Task<IActionResult> ObterStatus(Guid id, CancellationToken ct)
    {
        var status = await _obterStatus.Executar(id, ct);
        return Ok(status);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken ct)
    {
        var detalhe = await _obter.Executar(id, ct);

        return Ok(new
        {
            detalhe.Id,
            detalhe.VeiculoId,
            detalhe.TipoManutencao,
            detalhe.Status,
            detalhe.OrigemUltimaAtualizacaoStatus,
            detalhe.DataUltimaAtualizacaoStatus,
            detalhe.DataCriacao,
            detalhe.DataInicioExecucao,
            detalhe.DataFimExecucao,
            diagnostico = detalhe.Diagnostico is null
                ? null
                : new { detalhe.Diagnostico.Descricao, detalhe.Diagnostico.DataRegistro },
            orcamento = detalhe.Orcamento is null
                ? null
                : new
                {
                    detalhe.Orcamento.Id,
                    status = detalhe.Orcamento.Status,
                    detalhe.Orcamento.ValorTotal,
                    itensServico = detalhe.Orcamento.ItensServico.Select(x => new { x.ServicoId, x.ValorMaoDeObra }),
                    itensMaterial = detalhe.Orcamento.ItensMaterial.Select(x => new
                    {
                        tipo = x.Tipo,
                        x.MaterialId,
                        x.Quantidade,
                        x.ValorUnitario,
                        descricao = x.Descricao
                    })
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
