using Oficina.Domain.Oficina.Enums;

namespace Oficina.Application.DTO.Oficina;

public record CriarOrdemServicoRequest(
    Guid VeiculoId,
    TipoManutencao TipoManutencao,
    IReadOnlyList<Guid>? ServicoIds);
