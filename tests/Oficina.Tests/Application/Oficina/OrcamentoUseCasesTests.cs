using Moq;
using Oficina.Application.Abstractions.Notificacoes;
using Oficina.Application.Abstractions.Repositorios;
using Oficina.Application.UseCases.Oficina;
using Oficina.Domain.Oficina;
using Oficina.Domain.Oficina.Enums;
using Xunit;

namespace Oficina.Tests.Application.Oficina;

public class OrcamentoUseCasesTests
{
    [Fact]
    public async Task RecusarOrcamento_DeveFinalizarOsESolicitarRetirada()
    {
        var oficinaRepo = new Mock<IOficinaRepository>();
        var catalogo = new Mock<ICatalogoEstoqueRepository>();
        var notificador = new Mock<INotificadorCliente>();

        var os = OrdemServico.CriarCorretiva(Guid.NewGuid());

        await Assert.ThrowsAsync<ArgumentException>( () =>
           Task.Run(() => os.RegistrarDiagnostico("Barulho na suspensão", Enumerable.Empty<Guid>())));

        var orc = new Orcamento(os.Id, 50);
        
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
          Task.Run(() => os.VincularOrcamento(orc.Id)));

        oficinaRepo.Setup(x => x.ObterOrcamento(orc.Id, It.IsAny<CancellationToken>())).ReturnsAsync(orc);
        oficinaRepo.Setup(x => x.ObterOrdemServico(os.Id, It.IsAny<CancellationToken>())).ReturnsAsync(os);

        var uc = new RecusarOrcamentoUseCase(oficinaRepo.Object, notificador.Object);

        await uc.Executar(orc.Id, CancellationToken.None);

        Assert.Equal(StatusOrcamento.Recusado, orc.Status);
        Assert.Equal(StatusOrdemServico.Finalizada, os.Status);

        oficinaRepo.Verify(x => x.Salvar(It.IsAny<CancellationToken>()), Times.Once);
        notificador.Verify(x => x.NotificarOrcamentoRecusado(orc.Id, os.Id, It.IsAny<CancellationToken>()), Times.Once);
        catalogo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ObterOrcamento_DeveRetornarDto()
    {
        var oficinaRepo = new Mock<IOficinaRepository>();
        var orc = new Orcamento(Guid.NewGuid(), 120);

        oficinaRepo.Setup(x => x.ObterOrcamento(orc.Id, It.IsAny<CancellationToken>())).ReturnsAsync(orc);
        var uc = new ObterOrcamentoUseCase(oficinaRepo.Object);

        var dto = await uc.Executar(orc.Id, CancellationToken.None);

        Assert.Equal(orc.Id, dto.Id);
        Assert.Equal(orc.ValorTotal, dto.ValorTotal);
        Assert.Equal(orc.Status, dto.Status);
    }
}
