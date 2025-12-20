using Oficina.Domain.Cadastro.ValueObjects;
using Oficina.Domain.SharedKernel;

namespace Oficina.Domain.Cadastro;

public class Cliente : AgregadoRaiz
{
    private Cliente() { } // EF

    public Cliente(DocumentoCpfCnpj documento)
    {
        Documento = documento ?? throw new ArgumentNullException(nameof(documento));
    }

    public DocumentoCpfCnpj Documento { get; private set; } = default!;

    public void AlterarDocumento(DocumentoCpfCnpj documento)
    {
        Documento = documento ?? throw new ArgumentNullException(nameof(documento));
    }
}
