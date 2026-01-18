using Oficina.Application.Abstractions.Repositorios;
using Oficina.Application.Shared;
using Oficina.Domain.CatalogoEstoque;

namespace Oficina.Application.UseCases.CatalogoEstoque;

public class CadastrarServicoUseCase
{
    private readonly ICatalogoEstoqueRepository _repo;
    public CadastrarServicoUseCase(ICatalogoEstoqueRepository repo) => _repo = repo;

    public async Task<Guid> Executar(decimal maoDeObra, IEnumerable<(Guid id, int qtd)>? pecas, IEnumerable<(Guid id, int qtd)>? insumos, CancellationToken ct)
    {
        var servico = new Servico(maoDeObra);

        if (pecas != null)
            foreach (var (id, qtd) in pecas)
                servico.AdicionarPeca(id, qtd);

        if (insumos != null)
            foreach (var (id, qtd) in insumos)
                servico.AdicionarInsumo(id, qtd);

        await _repo.AdicionarServico(servico, ct);
        await _repo.Salvar(ct);
        return servico.Id;
    }
}

public class CadastrarPecaUseCase
{
    private readonly ICatalogoEstoqueRepository _repo;
    public CadastrarPecaUseCase(ICatalogoEstoqueRepository repo) => _repo = repo;

    public async Task<Guid> Executar(decimal precoUnitario, string descricao, CancellationToken ct)
    {
        var peca = new Peca(precoUnitario, descricao);
        await _repo.AdicionarPeca(peca, ct);

        // estoque inicial = 0
        await _repo.AdicionarEstoquePeca(new EstoquePeca(peca.Id, 0), ct);

        await _repo.Salvar(ct);
        return peca.Id;
    }

}




public class ObterPecaUseCase
{
    private readonly ICatalogoEstoqueRepository _repo;
    public ObterPecaUseCase(ICatalogoEstoqueRepository repo) => _repo = repo;

    public async Task<Peca> Executar(Guid id, CancellationToken ct)
        => await _repo.ObterPeca(id, ct) ?? throw new OficinaException("Peça não encontrada.", 404);
}

public class CadastrarInsumoUseCase
{
    private readonly ICatalogoEstoqueRepository _repo;
    public CadastrarInsumoUseCase(ICatalogoEstoqueRepository repo) => _repo = repo;

    public async Task<Guid> Executar(decimal precoUnitario, string descricao, CancellationToken ct)
    {
        var insumo = new Insumo(precoUnitario, descricao);
        await _repo.AdicionarInsumo(insumo, ct);

        // estoque inicial = 0
        await _repo.AdicionarEstoqueInsumo(new EstoqueInsumo(insumo.Id, 0), ct);

        await _repo.Salvar(ct);
        return insumo.Id;
    }
}

public class ObterInsumoUseCase
{
    private readonly ICatalogoEstoqueRepository _repo;
    public ObterInsumoUseCase(ICatalogoEstoqueRepository repo) => _repo = repo;

    public async Task<Insumo> Executar(Guid id, CancellationToken ct)
        => await _repo.ObterInsumo(id, ct) ?? throw new OficinaException("Insumo não encontrado.", 404);
}

public class AtualizarPecaUseCase
{
    private readonly ICatalogoEstoqueRepository _repo;
    public AtualizarPecaUseCase(ICatalogoEstoqueRepository repo) => _repo = repo;

    public async Task Executar(Guid id, decimal precoUnitario, string descricao, CancellationToken ct)
    {
        var peca = await _repo.ObterPeca(id, ct) ?? throw new OficinaException("Peça não encontrada.", 404);

        peca.DefinirDescricao(descricao);
        peca.DefinirPreco(precoUnitario);

        await _repo.Salvar(ct);
    }
}

public class AtualizarInsumoUseCase
{
    private readonly ICatalogoEstoqueRepository _repo;
    public AtualizarInsumoUseCase(ICatalogoEstoqueRepository repo) => _repo = repo;

    public async Task Executar(Guid id, decimal precoUnitario, string descricao, CancellationToken ct)
    {
        var insumo = await _repo.ObterInsumo(id, ct) ?? throw new OficinaException("Insumo não encontrado.", 404);

        insumo.DefinirDescricao(descricao);
        insumo.DefinirPreco(precoUnitario);

        await _repo.Salvar(ct);
    }
}
