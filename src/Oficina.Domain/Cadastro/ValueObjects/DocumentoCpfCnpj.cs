using Oficina.Domain.SharedKernel;

namespace Oficina.Domain.Cadastro.ValueObjects;

public sealed class DocumentoCpfCnpj
{
    public string Valor { get; private set; } = default!;

    private DocumentoCpfCnpj() { } // EF

    public DocumentoCpfCnpj(string valor)
    {
        Valor = Normalizar(valor);
        Validar(Valor);
    }

    private static string Normalizar(string valor) =>
        new string(valor.Where(char.IsDigit).ToArray());

    private static void Validar(string valorNormalizado)
    {
        if (string.IsNullOrWhiteSpace(valorNormalizado))
            throw new ArgumentException("Documento é obrigatório.");

        if (valorNormalizado.Length != 11 && valorNormalizado.Length != 14)
            throw new ArgumentException("Documento deve ter 11 (CPF) ou 14 (CNPJ) dígitos.");
    }
}
