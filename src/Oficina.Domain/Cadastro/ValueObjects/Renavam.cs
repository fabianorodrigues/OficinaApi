namespace Oficina.Domain.Cadastro.ValueObjects;

public class Renavam
{
    public string Valor { get; }

    private Renavam(string valorNormalizado)
    {
        Valor = valorNormalizado;
    }

    public static Renavam Criar(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("RENAVAM é obrigatório.");

        var digitos = new string(valor.Where(char.IsDigit).ToArray());
        if (digitos.Length != 11)
            throw new ArgumentException("RENAVAM inválido.");

        return new Renavam(digitos);
    }
}
