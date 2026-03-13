using Oficina.Domain.Oficina.Enums;

namespace Oficina.Application.DTO.Oficina;

public record AtualizarStatusExternoOrdemServicoRequest(StatusOrdemServico Status, string Origem);
