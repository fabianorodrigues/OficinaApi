using Oficina.Domain.SharedKernel;

namespace Oficina.Domain.Cadastro.ValueObjects;

public class DocumentoCpfCnpj
{
    public string Valor { get; }

    private DocumentoCpfCnpj(string valorNormalizado)
    {
        Valor = valorNormalizado;
    }

    public static DocumentoCpfCnpj Criar(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("CPF/CNPJ é obrigatório.");

        var digitos = new string(valor.Where(char.IsDigit).ToArray());
        if (digitos.Length is not (11 or 14))
            throw new ArgumentException("CPF/CNPJ inválido.");

        return new DocumentoCpfCnpj(digitos);
    }

}
