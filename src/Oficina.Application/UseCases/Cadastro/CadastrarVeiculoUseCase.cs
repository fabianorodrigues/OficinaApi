using Oficina.Application.Abstractions.Repositorios;
using Oficina.Application.Shared;
using Oficina.Domain.Cadastro;
using Oficina.Domain.Cadastro.ValueObjects;

namespace Oficina.Application.UseCases.Cadastro;

public class CadastrarVeiculoUseCase
{
    private readonly ICadastroRepository _repo;

    public CadastrarVeiculoUseCase(ICadastroRepository repo) => _repo = repo;

    public async Task<Guid> Executar(Guid clienteId, string placa, string renavam, CancellationToken ct)
    {
        var cliente = await _repo.ObterCliente(clienteId, ct);
        if (cliente is null)
            throw new OficinaException("Cliente não encontrado.", 404);

        var placaVo = new Placa(placa);

        if (await _repo.ExisteVeiculoPorPlaca(placaVo.Valor, ct))
            throw new OficinaException("Já existe veículo cadastrado com esta placa.", 409);

        var veiculo = new Veiculo(clienteId, placaVo, new Renavam(renavam));

        await _repo.AdicionarVeiculo(veiculo, ct);
        await _repo.Salvar(ct);

        return veiculo.Id;
    }
}
