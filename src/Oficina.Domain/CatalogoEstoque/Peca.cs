using Oficina.Domain.SharedKernel;

namespace Oficina.Domain.CatalogoEstoque;

public class Peca : AgregadoRaiz
{
    private Peca() { } // EF

    public Peca(decimal precoUnitario)
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
