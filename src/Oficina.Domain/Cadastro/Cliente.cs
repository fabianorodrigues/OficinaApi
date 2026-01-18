using Oficina.Domain.Cadastro.ValueObjects;
using Oficina.Domain.SharedKernel;

namespace Oficina.Domain.Cadastro;

public class Cliente : AgregadoRaiz
{
    private Cliente() { } // EF

    public Cliente(DocumentoCpfCnpj documento, string nome, Contato contato)
    {
        Nome = nome;
        Documento = documento ?? throw new ArgumentNullException(nameof(documento));
        Contato = contato;
    }
    public string Nome { get; private set; }
    public DocumentoCpfCnpj Documento { get; private set; } = default!;
    public Contato Contato { get; private set; }

    public void AlterarDocumento(DocumentoCpfCnpj documento)
    {
        Documento = documento ?? throw new ArgumentNullException(nameof(documento));
    }

    public void AlterarContato(Contato contato)
    {
        Contato = contato ?? throw new ArgumentNullException(nameof(contato));
    }

    private void DefinirNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório.");

        Nome = nome.Trim();
    }
}
