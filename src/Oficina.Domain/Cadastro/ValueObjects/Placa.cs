using System.Text.RegularExpressions;

namespace Oficina.Domain.Cadastro.ValueObjects;

public class Placa 
{
    public string Valor { get; }

    private Placa(string valorNormalizado)
    {
        Valor = valorNormalizado;
    }

    public static Placa Criar(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Placa é obrigatória.");

        var p = valor.Trim().ToUpperInvariant().Replace("-", "");

        var antigo = Regex.IsMatch(p, "^[A-Z]{3}[0-9]{4}$");
        var mercosul = Regex.IsMatch(p, "^[A-Z]{3}[0-9]{1}[A-Z]{1}[0-9]{2}$");

        if (!antigo && !mercosul)
            throw new ArgumentException("Placa inválida.");

        return new Placa(p);
    }
}
