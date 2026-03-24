namespace Oficina.Application.DTO.CatalogoEstoque;

public sealed class EstoquePecaResponse
{
    public Guid PecaId { get; init; }
    public string? Descricao { get; init; }
    public int Quantidade { get; init; }
}

public sealed class EstoqueInsumoResponse
{
    public Guid InsumoId { get; init; }
    public string? Descricao { get; init; }
    public int Quantidade { get; init; }
}
