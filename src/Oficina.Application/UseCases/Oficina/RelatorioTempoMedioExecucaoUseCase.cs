using Oficina.Application.Abstractions.Repositorios;

namespace Oficina.Application.UseCases.Oficina;

public class RelatorioTempoMedioExecucaoUseCase
{
    private readonly IOficinaRepository _repo;
    public RelatorioTempoMedioExecucaoUseCase(IOficinaRepository repo) => _repo = repo;

    public async Task<TimeSpan?> Executar(CancellationToken ct)
    {
        var ordens = await _repo.ListarOrdensServico(ct);

        var duracoes = ordens
            .Where(o => o.DataInicioExecucao.HasValue && o.DataFimExecucao.HasValue)
            .Select(o => o.DataFimExecucao!.Value - o.DataInicioExecucao!.Value)
            .ToList();

        if (duracoes.Count == 0) return null;

        var mediaTicks = (long)duracoes.Average(d => d.Ticks);
        return TimeSpan.FromTicks(mediaTicks);
    }
}
