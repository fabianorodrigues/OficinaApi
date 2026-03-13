using Oficina.Application.Abstractions.Repositorios;
using Oficina.Application.Shared;
using Oficina.Domain.Oficina;
using Oficina.Domain.Oficina.Enums;

namespace Oficina.Application.UseCases.Oficina;

public class ObterOrdemServicoUseCase
{
    private readonly IOficinaRepository _repo;
    public ObterOrdemServicoUseCase(IOficinaRepository repo) => _repo = repo;

    public async Task<(OrdemServico os, Orcamento? orcamento)> Executar(Guid id, CancellationToken ct)
    {
        var os = await _repo.ObterOrdemServico(id, ct) ?? throw new OficinaException("Ordem de serviço não encontrada.", 404);
        var orc = os.OrcamentoId is null ? null : await _repo.ObterOrcamento(os.OrcamentoId.Value, ct);
        return (os, orc);
    }
}

public class ObterStatusOrdemServicoUseCase
{
    private readonly IOficinaRepository _repo;
    public ObterStatusOrdemServicoUseCase(IOficinaRepository repo) => _repo = repo;

    public async Task<StatusOrdemServico> Executar(Guid ordemServicoId, CancellationToken ct)
    {
        var os = await _repo.ObterOrdemServico(ordemServicoId, ct)
            ?? throw new OficinaException("Ordem de serviço não encontrada.", 404);

        return os.Status;
    }
}

public class ListarOrdensServicoUseCase
{
    private readonly IOficinaRepository _repo;
    public ListarOrdensServicoUseCase(IOficinaRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<OrdemServico>> Executar(CancellationToken ct)
    {
        var ordens = await _repo.ListarOrdensServico(ct);

        return ordens
            .Where(x => x.Status != StatusOrdemServico.Finalizada && x.Status != StatusOrdemServico.Entregue)
            .OrderBy(x => Prioridade(x.Status))
            .ThenBy(x => x.DataCriacao)
            .ToList();
    }

    private static int Prioridade(StatusOrdemServico status) => status switch
    {
        StatusOrdemServico.EmExecucao => 1,
        StatusOrdemServico.AguardandoAprovacao => 2,
        StatusOrdemServico.EmDiagnostico => 3,
        StatusOrdemServico.Recebida => 4,
        _ => 99
    };
}

public class AtualizarStatusExternoOrdemServicoUseCase
{
    private readonly IOficinaRepository _repo;

    public AtualizarStatusExternoOrdemServicoUseCase(IOficinaRepository repo)
    {
        _repo = repo;
    }

    public async Task Executar(Guid ordemServicoId, StatusOrdemServico status, CancellationToken ct)
    {
        var os = await _repo.ObterOrdemServico(ordemServicoId, ct)
            ?? throw new OficinaException("Ordem de serviço não encontrada.", 404);

        os.AtualizarStatusExterno(status);
        await _repo.Salvar(ct);
    }
}

public class FinalizarOrdemServicoUseCase
{
    private readonly IOficinaRepository _repo;
    public FinalizarOrdemServicoUseCase(IOficinaRepository repo) => _repo = repo;

    public async Task Executar(Guid ordemServicoId, CancellationToken ct)
    {
        var os = await _repo.ObterOrdemServico(ordemServicoId, ct)
                 ?? throw new OficinaException("Ordem de serviço não encontrada.", 404);

        os.Finalizar();
        await _repo.Salvar(ct);
    }
}

public class EntregarOrdemServicoUseCase
{
    private readonly IOficinaRepository _repo;
    public EntregarOrdemServicoUseCase(IOficinaRepository repo) => _repo = repo;

    public async Task Executar(Guid ordemServicoId, CancellationToken ct)
    {
        var os = await _repo.ObterOrdemServico(ordemServicoId, ct)
                 ?? throw new OficinaException("Ordem de serviço não encontrada.", 404);

        os.MarcarEntregue();
        await _repo.Salvar(ct);
    }
}
