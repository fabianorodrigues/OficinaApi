using Oficina.Domain.Cadastro;

namespace Oficina.Application.Abstractions.Repositorios;

public interface ICadastroRepository
{
    Task<Cliente?> ObterCliente(Guid id, CancellationToken ct);
    Task<bool> ExisteClientePorDocumento(string cpfCnpjNormalizado, CancellationToken ct);
    Task AdicionarCliente(Cliente cliente, CancellationToken ct);

    Task<Veiculo?> ObterVeiculo(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Veiculo>> ListarVeiculosPorCliente(Guid clienteId, CancellationToken ct);
    Task<bool> ExisteVeiculoPorPlaca(string placaNormalizada, CancellationToken ct);
    Task AdicionarVeiculo(Veiculo veiculo, CancellationToken ct);

    Task Salvar(CancellationToken ct);
}
