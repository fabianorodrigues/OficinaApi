using Oficina.Application.Abstractions.Repositorios;
using Oficina.Application.Shared;
using Oficina.Domain.Oficina;

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

public class ListarOrdensServicoUseCase
{
    private readonly IOficinaRepository _repo;
    public ListarOrdensServicoUseCase(IOficinaRepository repo) => _repo = repo;

    public Task<IReadOnlyList<OrdemServico>> Executar(CancellationToken ct)
        => _repo.ListarOrdensServico(ct);
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
