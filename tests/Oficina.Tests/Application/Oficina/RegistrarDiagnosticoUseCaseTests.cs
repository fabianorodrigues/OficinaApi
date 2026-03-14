using Moq;
using Oficina.Application.Abstractions.Notificacoes;
using Oficina.Application.Abstractions.Repositorios;
using Oficina.Application.Shared;
using Oficina.Application.UseCases.Oficina;
using Oficina.Domain.Oficina;
using Xunit;

namespace Oficina.Tests.Application.Oficina;

public class RegistrarDiagnosticoUseCaseTests
{
    [Fact]
    public async Task RegistrarDiagnostico_Deve_lancar_quando_os_nao_existe()
    {
        var repoOficina = new Mock<IOficinaRepository>();
        repoOficina.Setup(x => x.ObterOrdemServico(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrdemServico?)null);

        var useCase = new RegistrarDiagnosticoUseCase(
            repoOficina.Object,
            Mock.Of<ICatalogoEstoqueRepository>(),
            Mock.Of<INotificadorCliente>());

        await Assert.ThrowsAsync<OficinaException>(() =>
            useCase.Executar(Guid.NewGuid(), "x", new[] { Guid.NewGuid() }, CancellationToken.None));
    }
}
