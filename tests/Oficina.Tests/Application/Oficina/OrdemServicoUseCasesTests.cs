using Moq;
using Oficina.Application.Abstractions.Repositorios;
using Oficina.Application.UseCases.Oficina;
using Oficina.Domain.Oficina;
using Oficina.Domain.Oficina.Enums;
using Xunit;

namespace Oficina.Tests.Application.Oficina;

public class OrdemServicoUseCasesTests
{
    [Fact]
    public async Task ListarOrdensServico_DeveFiltrarFinalizadasEOrdenarPorPrioridadeEData()
    {
        var repo = new Mock<IOficinaRepository>();

        var recebida = OrdemServico.CriarCorretiva(Guid.NewGuid());
        var aguardando = OrdemServico.CriarPreventiva(Guid.NewGuid(), new[] { Guid.NewGuid() });
        aguardando.AvancarParaFluxoInicial();
        var emExecucao = OrdemServico.CriarPreventiva(Guid.NewGuid(), new[] { Guid.NewGuid() });
        emExecucao.AvancarParaFluxoInicial();
        var orcExec = new Orcamento(emExecucao.Id, 10m);
        emExecucao.VincularOrcamento(orcExec.Id);
        orcExec.Aprovar();
        emExecucao.IniciarExecucao(orcExec);

        var finalizada = OrdemServico.CriarPreventiva(Guid.NewGuid(), new[] { Guid.NewGuid() });
        finalizada.AvancarParaFluxoInicial();
        var orcFin = new Orcamento(finalizada.Id, 10m);
        finalizada.VincularOrcamento(orcFin.Id);
        orcFin.Aprovar();
        finalizada.IniciarExecucao(orcFin);
        finalizada.Finalizar();

        repo.Setup(x => x.ListarOrdensServico(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OrdemServico> { recebida, finalizada, aguardando, emExecucao });

        var useCase = new ListarOrdensServicoUseCase(repo.Object);

        var resultado = await useCase.Executar(CancellationToken.None);

        Assert.Equal(3, resultado.Count);
        Assert.Equal(StatusOrdemServico.EmExecucao, resultado[0].Status);
        Assert.Equal(StatusOrdemServico.AguardandoAprovacao, resultado[1].Status);
        Assert.Equal(StatusOrdemServico.Recebida, resultado[2].Status);
    }

    [Fact]
    public async Task AtualizarStatusExterno_DevePersistirTransicaoValida()
    {
        var repo = new Mock<IOficinaRepository>();
        var os = OrdemServico.CriarCorretiva(Guid.NewGuid());

        repo.Setup(x => x.ObterOrdemServico(os.Id, It.IsAny<CancellationToken>())).ReturnsAsync(os);

        var useCase = new AtualizarStatusExternoOrdemServicoUseCase(repo.Object);
        await useCase.Executar(os.Id, StatusOrdemServico.EmDiagnostico, CancellationToken.None);

        Assert.Equal(StatusOrdemServico.EmDiagnostico, os.Status);
        repo.Verify(x => x.Salvar(It.IsAny<CancellationToken>()), Times.Once);
    }
}
