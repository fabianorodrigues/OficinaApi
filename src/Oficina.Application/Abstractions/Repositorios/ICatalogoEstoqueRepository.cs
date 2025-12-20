using Oficina.Domain.CatalogoEstoque;

namespace Oficina.Application.Abstractions.Repositorios;

public interface ICatalogoEstoqueRepository
{
    Task<Servico?> ObterServico(Guid id, CancellationToken ct);
    Task AdicionarServico(Servico servico, CancellationToken ct);

    Task<Peca?> ObterPeca(Guid id, CancellationToken ct);
    Task AdicionarPeca(Peca peca, CancellationToken ct);

    Task<Insumo?> ObterInsumo(Guid id, CancellationToken ct);
    Task AdicionarInsumo(Insumo insumo, CancellationToken ct);

    Task<EstoquePeca?> ObterEstoquePeca(Guid pecaId, CancellationToken ct);
    Task<EstoqueInsumo?> ObterEstoqueInsumo(Guid insumoId, CancellationToken ct);
    Task AdicionarEstoquePeca(EstoquePeca estoque, CancellationToken ct);
    Task AdicionarEstoqueInsumo(EstoqueInsumo estoque, CancellationToken ct);

    Task Salvar(CancellationToken ct);
}
