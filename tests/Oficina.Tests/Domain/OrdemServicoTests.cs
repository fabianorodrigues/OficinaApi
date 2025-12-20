using Oficina.Domain.Oficina;
using Oficina.Domain.Oficina.Enums;
using Xunit;

namespace Oficina.Tests.Domain;

public class OrdemServicoTests
{
    [Fact]
    public void CriarPreventiva_DeveIniciarAguardandoAprovacao()
    {
        var os = OrdemServico.CriarPreventiva(Guid.NewGuid(), new[] { Guid.NewGuid() });
        Assert.Equal(StatusOrdemServico.AguardandoAprovacao, os.Status);
        Assert.Equal(TipoManutencao.Preventiva, os.TipoManutencao);
        Assert.Single(os.ItensServico);
    }

    [Fact]
    public void CriarCorretiva_DeveIniciarEmDiagnostico()
    {
        var os = OrdemServico.CriarCorretiva(Guid.NewGuid());
        Assert.Equal(StatusOrdemServico.EmDiagnostico, os.Status);
        Assert.Equal(TipoManutencao.Corretiva, os.TipoManutencao);
    }

    [Fact]
    public void RegistrarDiagnostico_DeveMoverParaAguardandoAprovacao()
    {
        var os = OrdemServico.CriarCorretiva(Guid.NewGuid());
        os.RegistrarDiagnostico("Problema", new[] { Guid.NewGuid() });

        Assert.NotNull(os.Diagnostico);
        Assert.Equal(StatusOrdemServico.AguardandoAprovacao, os.Status);
        Assert.Single(os.ItensServico);
    }

    [Fact]
    public void IniciarExecucao_SoComOrcamentoAprovado()
    {
        var os = OrdemServico.CriarPreventiva(Guid.NewGuid(), new[] { Guid.NewGuid() });

        var orc = new Orcamento(os.Id, 10);
        os.VincularOrcamento(orc.Id);

        Assert.Throws<InvalidOperationException>(() => os.IniciarExecucao(orc));

        orc.Aprovar();
        os.IniciarExecucao(orc);

        Assert.Equal(StatusOrdemServico.EmExecucao, os.Status);
        Assert.NotNull(os.DataInicioExecucao);
    }

    [Fact]
    public void Finalizar_AtualizaDataFim()
    {
        var os = OrdemServico.CriarPreventiva(Guid.NewGuid(), new[] { Guid.NewGuid() });
        var orc = new Orcamento(os.Id, 10);
        os.VincularOrcamento(orc.Id);
        orc.Aprovar();
        os.IniciarExecucao(orc);

        os.Finalizar();
        Assert.Equal(StatusOrdemServico.Finalizada, os.Status);
        Assert.NotNull(os.DataFimExecucao);
    }

    [Fact]
    public void RecusaFinalizaSemExecucao()
    {
        var os = OrdemServico.CriarPreventiva(Guid.NewGuid(), new[] { Guid.NewGuid() });
        var orc = new Orcamento(os.Id, 10);
        os.VincularOrcamento(orc.Id);

        orc.Recusar();
        os.FinalizarPorRecusaOrcamento(orc);

        Assert.Equal(StatusOrdemServico.Finalizada, os.Status);
        Assert.Null(os.DataInicioExecucao);
        Assert.Null(os.DataFimExecucao);
    }
}
