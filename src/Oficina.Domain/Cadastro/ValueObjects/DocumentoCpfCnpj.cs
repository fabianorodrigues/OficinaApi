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
        new string((valor ?? string.Empty).Where(char.IsDigit).ToArray());

    private static void Validar(string valorNormalizado)
    {
        if (string.IsNullOrWhiteSpace(valorNormalizado))
            throw new ArgumentException("Documento é obrigatório.");

        if (valorNormalizado.Length == 11)
        {
            if (!CpfValido(valorNormalizado))
                throw new ArgumentException("CPF inválido.");
            return;
        }

        if (valorNormalizado.Length == 14)
        {
            if (!CnpjValido(valorNormalizado))
                throw new ArgumentException("CNPJ inválido.");
            return;
        }

        throw new ArgumentException("Documento deve ter 11 (CPF) ou 14 (CNPJ) dígitos.");
    }

    private static bool CpfValido(string cpf)
    {
        if (TodosDigitosIguais(cpf)) return false;

        var primeiroDigito = CalcularDigito(cpf[..9], 10);
        var segundoDigito = CalcularDigito(cpf[..10], 11);

        return cpf[9] - '0' == primeiroDigito && cpf[10] - '0' == segundoDigito;
    }

    private static bool CnpjValido(string cnpj)
    {
        if (TodosDigitosIguais(cnpj)) return false;

        var primeiroDigito = CalcularDigitoCnpj(cnpj[..12], new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 });
        var segundoDigito = CalcularDigitoCnpj(cnpj[..13], new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 });

        return cnpj[12] - '0' == primeiroDigito && cnpj[13] - '0' == segundoDigito;
    }

    private static bool TodosDigitosIguais(string valor)
        => valor.All(c => c == valor[0]);

    private static int CalcularDigito(string baseNumerica, int pesoInicial)
    {
        var soma = 0;
        for (var i = 0; i < baseNumerica.Length; i++)
            soma += (baseNumerica[i] - '0') * (pesoInicial - i);

        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }

    private static int CalcularDigitoCnpj(string baseNumerica, IReadOnlyList<int> pesos)
    {
        var soma = 0;
        for (var i = 0; i < baseNumerica.Length; i++)
            soma += (baseNumerica[i] - '0') * pesos[i];

        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }
}
