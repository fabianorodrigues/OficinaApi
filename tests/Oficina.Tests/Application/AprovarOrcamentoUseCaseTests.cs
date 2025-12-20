using Moq;
using Oficina.Application.Abstractions.Repositorios;
using Oficina.Application.UseCases.Oficina;
using Oficina.Domain.CatalogoEstoque;
using Oficina.Domain.Oficina;
using Oficina.Domain.Oficina.Enums;
using Xunit;

namespace Oficina.Tests.Application;

public class AprovarOrcamentoUseCaseTests
{
    [Fact]
    public async Task Aprovar_DeveBaixarEstoqueEIniciarExecucao()
    {
        var oficinaRepo = new Mock<IOficinaRepository>();
        var estoqueRepo = new Mock<ICatalogoEstoqueRepository>();

        var os = OrdemServico.CriarPreventiva(Guid.NewGuid(), new[] { Guid.NewGuid() });

        var materialId = Guid.NewGuid();
        var orc = new Orcamento(os.Id, 100);
        orc.DefinirItensMaterial(new[]
        {
            new OrcamentoItemMaterial(TipoMaterial.Peca, materialId, 2, 10)
        });

        os.VincularOrcamento(orc.Id);

        oficinaRepo.Setup(x => x.ObterOrcamento(orc.Id, It.IsAny<CancellationToken>())).ReturnsAsync(orc);
        oficinaRepo.Setup(x => x.ObterOrdemServico(os.Id, It.IsAny<CancellationToken>())).ReturnsAsync(os);

        var estoque = new EstoquePeca(materialId, 0);
        estoqueRepo.Setup(x => x.ObterEstoquePeca(materialId, It.IsAny<CancellationToken>())).ReturnsAsync(estoque);

        var uc = new AprovarOrcamentoUseCase(oficinaRepo.Object, estoqueRepo.Object);

        await uc.Executar(orc.Id, CancellationToken.None);

        Assert.Equal(StatusOrcamento.Aprovado, orc.Status);
        Assert.Equal(StatusOrdemServico.EmExecucao, os.Status);
        Assert.Equal(-2, estoque.Quantidade);

        estoqueRepo.Verify(x => x.Salvar(It.IsAny<CancellationToken>()), Times.Once);
        oficinaRepo.Verify(x => x.Salvar(It.IsAny<CancellationToken>()), Times.Once);
    }
}
