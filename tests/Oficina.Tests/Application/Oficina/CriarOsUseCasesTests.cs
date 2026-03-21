using Moq;
using Oficina.Application.Abstractions.Notificacoes;
using Oficina.Application.Abstractions.Repositorios;
using Oficina.Application.Shared;
using Oficina.Application.UseCases.Oficina;
using Xunit;

namespace Oficina.Tests.Application.Oficina;

public class CriarOsUseCasesTests
{

    [Fact]
    public async Task CriarOsPreventiva_Deve_lancar_quando_veiculo_nao_existe()
    {
        var repoCadastro = new Mock<ICadastroRepository>();
        repoCadastro.Setup(x => x.ExisteVeiculoPorPlaca(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var useCase = new CriarOsPreventivaUseCase(
            repoCadastro.Object,
            Mock.Of<ICatalogoEstoqueRepository>(),
            Mock.Of<IOficinaRepository>(),
            Mock.Of<INotificadorCliente>());

        await Assert.ThrowsAsync<OficinaException>(() =>
            useCase.Executar(Guid.NewGuid(), new[] { Guid.NewGuid() }, CancellationToken.None));
    }
}
