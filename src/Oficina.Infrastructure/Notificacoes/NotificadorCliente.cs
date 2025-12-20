using Microsoft.Extensions.Logging;
using Oficina.Application.Abstractions.Notificacoes;

namespace Oficina.Infrastructure.Notificacoes;

// MVP: notificação gerando apenas um log
public class NotificadorCliente : INotificadorCliente
{
    private readonly ILogger<NotificadorCliente> _logger;

    public NotificadorCliente(ILogger<NotificadorCliente> logger)
    {
        _logger = logger;
    }

    public Task NotificarOrcamentoCriado(Guid orcamentoId, Guid ordemServicoId, CancellationToken ct)
    {
        _logger.LogInformation("Notificação: orçamento criado {OrcamentoId} para OS {OrdemServicoId}.", orcamentoId, ordemServicoId);
        return Task.CompletedTask;
    }

    public Task NotificarOrcamentoRecusado(Guid orcamentoId, Guid ordemServicoId, CancellationToken ct)
    {
        _logger.LogInformation("Notificação: orçamento recusado {OrcamentoId} para OS {OrdemServicoId}. Cliente deve retirar o veículo.", orcamentoId, ordemServicoId);
        return Task.CompletedTask;
    }
}
