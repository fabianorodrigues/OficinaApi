using Oficina.Application.Abstractions.Repositorios;
using Oficina.Domain.Cadastro;
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

    public async Task<Guid> Executar(decimal precoUnitario, CancellationToken ct)
    {
        var peca = new Peca(precoUnitario);
        await _repo.AdicionarPeca(peca, ct);

        // estoque inicial = 0
        await _repo.AdicionarEstoquePeca(new EstoquePeca(peca.Id, 0), ct);

        await _repo.Salvar(ct);
        return peca.Id;
    }
}

public class CadastrarInsumoUseCase
{
    private readonly ICatalogoEstoqueRepository _repo;
    public CadastrarInsumoUseCase(ICatalogoEstoqueRepository repo) => _repo = repo;

    public async Task<Guid> Executar(decimal precoUnitario, CancellationToken ct)
    {
        var insumo = new Insumo(precoUnitario);
        await _repo.AdicionarInsumo(insumo, ct);

        // estoque inicial = 0
        await _repo.AdicionarEstoqueInsumo(new EstoqueInsumo(insumo.Id, 0), ct);

        await _repo.Salvar(ct);
        return insumo.Id;
    }
}
