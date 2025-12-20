using Oficina.Application.Abstractions.Repositorios;
using Oficina.Application.Shared;

namespace Oficina.Application.UseCases.CatalogoEstoque;

public class ObterEstoquePecaUseCase
{
    private readonly ICatalogoEstoqueRepository _repo;
    public ObterEstoquePecaUseCase(ICatalogoEstoqueRepository repo) => _repo = repo;

    public async Task<int> Executar(Guid pecaId, CancellationToken ct)
    {
        var estoque = await _repo.ObterEstoquePeca(pecaId, ct);
        if (estoque is null) throw new OficinaException("Estoque da peça não encontrado.", 404);
        return estoque.Quantidade;
    }
}

public class ObterEstoqueInsumoUseCase
{
    private readonly ICatalogoEstoqueRepository _repo;
    public ObterEstoqueInsumoUseCase(ICatalogoEstoqueRepository repo) => _repo = repo;

    public async Task<int> Executar(Guid insumoId, CancellationToken ct)
    {
        var estoque = await _repo.ObterEstoqueInsumo(insumoId, ct);
        if (estoque is null) throw new OficinaException("Estoque do insumo não encontrado.", 404);
        return estoque.Quantidade;
    }
}

public class AjustarEstoquePecaUseCase
{
    private readonly ICatalogoEstoqueRepository _repo;
    public AjustarEstoquePecaUseCase(ICatalogoEstoqueRepository repo) => _repo = repo;

    public async Task Executar(Guid pecaId, int quantidade, CancellationToken ct)
    {
        var estoque = await _repo.ObterEstoquePeca(pecaId, ct);
        if (estoque is null) throw new OficinaException("Estoque da peça não encontrado.", 404);

        estoque.Ajustar(quantidade);
        await _repo.Salvar(ct);
    }
}

public class AjustarEstoqueInsumoUseCase
{
    private readonly ICatalogoEstoqueRepository _repo;
    public AjustarEstoqueInsumoUseCase(ICatalogoEstoqueRepository repo) => _repo = repo;

    public async Task Executar(Guid insumoId, int quantidade, CancellationToken ct)
    {
        var estoque = await _repo.ObterEstoqueInsumo(insumoId, ct);
        if (estoque is null) throw new OficinaException("Estoque do insumo não encontrado.", 404);

        estoque.Ajustar(quantidade);
        await _repo.Salvar(ct);
    }
}
