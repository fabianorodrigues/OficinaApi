using Oficina.Application.Abstractions.Repositorios;
using Oficina.Application.Shared;
using Oficina.Domain.Oficina.Enums;

namespace Oficina.Application.UseCases.Oficina;

public class ClassificarOrdemServicoUseCase
{
    private readonly IOficinaRepository _repo;

    public ClassificarOrdemServicoUseCase(IOficinaRepository repo)
    {
        _repo = repo;
    }

    public async Task Executar(Guid ordemServicoId, string tipoManutencao, CancellationToken ct)
    {
        var os = await _repo.ObterOrdemServico(ordemServicoId, ct)
                 ?? throw new OficinaException("Ordem de serviço não encontrada.", 404);

        if (!Enum.TryParse<TipoManutencao>(tipoManutencao, true, out var tipo) || tipo == TipoManutencao.NaoClassificada)
            throw new OficinaException("Tipo de manutenção inválido.", 400);

        os.Classificar(tipo);
        await _repo.Salvar(ct);
    }
}
