using Oficina.Domain.Oficina.Enums;
using Oficina.Domain.Oficina.ValueObjects;
using Oficina.Domain.SharedKernel;

namespace Oficina.Domain.Oficina;

public class OrdemServico : AgregadoRaiz
{
    private readonly List<ItemServicoOs> _itensServico = [];

    private OrdemServico() { } // EF

    private OrdemServico(Guid veiculoId, TipoManutencao tipo)
    {
        if (veiculoId == Guid.Empty) throw new ArgumentException("Veículo inválido.");

        VeiculoId = veiculoId;
        TipoManutencao = tipo;
        Status = tipo == TipoManutencao.Corretiva
            ? StatusOrdemServico.EmDiagnostico
            : StatusOrdemServico.AguardandoAprovacao;

        DataCriacao = DateTimeOffset.UtcNow;
    }

    public Guid VeiculoId { get; private set; }
    public TipoManutencao TipoManutencao { get; private set; }
    public StatusOrdemServico Status { get; private set; }

    public DateTimeOffset DataCriacao { get; private set; }
    public DateTimeOffset? DataInicioExecucao { get; private set; }
    public DateTimeOffset? DataFimExecucao { get; private set; }

    public Guid? OrcamentoId { get; private set; }
    public Diagnostico? Diagnostico { get; private set; }

    public IReadOnlyCollection<ItemServicoOs> ItensServico => _itensServico;

    public static OrdemServico CriarPreventiva(Guid veiculoId, IEnumerable<Guid> servicoIds)
    {
        var lista = servicoIds?.Where(x => x != Guid.Empty).Distinct().ToList() ?? [];
        if (lista.Count == 0) throw new ArgumentException("OS preventiva exige ao menos 1 serviço.");

        var os = new OrdemServico(veiculoId, TipoManutencao.Preventiva);
        foreach (var id in lista) os._itensServico.Add(new ItemServicoOs(id));
        return os;
    }

    public static OrdemServico CriarCorretiva(Guid veiculoId)
        => new OrdemServico(veiculoId, TipoManutencao.Corretiva);

    public void RegistrarDiagnostico(string descricao, IEnumerable<Guid> servicoIds)
    {
        if (TipoManutencao != TipoManutencao.Corretiva)
            throw new InvalidOperationException("Diagnóstico só existe em OS corretiva.");

        if (Status != StatusOrdemServico.EmDiagnostico)
            throw new InvalidOperationException("OS não está em diagnóstico.");

        Diagnostico = new Diagnostico(descricao);

        var lista = servicoIds?.Where(x => x != Guid.Empty).Distinct().ToList() ?? [];
        if (lista.Count == 0) throw new ArgumentException("Diagnóstico exige ao menos 1 serviço identificado.");

        Status = StatusOrdemServico.AguardandoAprovacao;
    }

    public void VincularOrcamento(Guid orcamentoId)
    {
        if (orcamentoId == Guid.Empty) throw new ArgumentException("Orçamento inválido.");

        if (Status != StatusOrdemServico.AguardandoAprovacao)
            throw new InvalidOperationException("Orçamento só pode ser vinculado quando aguardando aprovação.");

        OrcamentoId = orcamentoId;
    }

    public void IniciarExecucao(Orcamento orcamento)
    {
        if (orcamento is null) throw new ArgumentNullException(nameof(orcamento));
        if (orcamento.OrdemServicoId != Id)
            throw new InvalidOperationException("Orçamento não corresponde à OS.");

        if (orcamento.Status != StatusOrcamento.Aprovado)
            throw new InvalidOperationException("Execução só pode iniciar após aprovação do orçamento.");

        if (Status != StatusOrdemServico.AguardandoAprovacao)
            throw new InvalidOperationException("OS não está aguardando aprovação.");

        Status = StatusOrdemServico.EmExecucao;
        DataInicioExecucao = DateTimeOffset.UtcNow;
    }

    public void Finalizar()
    {
        if (Status != StatusOrdemServico.EmExecucao)
            throw new InvalidOperationException("OS só pode ser finalizada após execução.");

        Status = StatusOrdemServico.Finalizada;
        DataFimExecucao = DateTimeOffset.UtcNow;
    }

    public void MarcarEntregue()
    {
        if (Status != StatusOrdemServico.Finalizada)
            throw new InvalidOperationException("OS só pode ser entregue após finalização.");

        Status = StatusOrdemServico.Entregue;
    }

    public void FinalizarPorRecusaOrcamento(Orcamento orcamento)
    {
        if (orcamento is null) throw new ArgumentNullException(nameof(orcamento));
        if (orcamento.OrdemServicoId != Id)
            throw new InvalidOperationException("Orçamento não corresponde à OS.");

        if (orcamento.Status != StatusOrcamento.Recusado)
            throw new InvalidOperationException("OS só finaliza por recusa quando o orçamento estiver recusado.");

        Status = StatusOrdemServico.Finalizada;
        // sem execução; sem cobrança do diagnóstico (fora do sistema)
    }
}
