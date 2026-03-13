using Oficina.Domain.Oficina.Enums;

namespace Oficina.Application.DTO.Oficina;

public record NotificarOrcamentoRequest(Guid OrcamentoId, StatusOrcamento Status);
