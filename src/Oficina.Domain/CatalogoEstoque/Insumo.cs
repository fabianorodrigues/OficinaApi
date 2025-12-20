using Oficina.Domain.SharedKernel;

namespace Oficina.Domain.CatalogoEstoque;

public class Insumo : AgregadoRaiz
{
    private Insumo() { } // EF

    public Insumo(decimal precoUnitario)
    {
        if (precoUnitario < 0) throw new ArgumentOutOfRangeException(nameof(precoUnitario));
        PrecoUnitario = precoUnitario;
    }

    public decimal PrecoUnitario { get; private set; }

    public void DefinirPreco(decimal precoUnitario)
    {
        if (precoUnitario < 0) throw new ArgumentOutOfRangeException(nameof(precoUnitario));
        PrecoUnitario = precoUnitario;
    }
}
