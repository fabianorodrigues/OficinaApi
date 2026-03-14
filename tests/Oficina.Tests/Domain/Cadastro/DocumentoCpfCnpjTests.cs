using Oficina.Domain.Cadastro.ValueObjects;
using Xunit;

namespace Oficina.Tests.Domain.Cadastro;

public class DocumentoCpfCnpjTests
{
    [Theory]
    [InlineData("123.456.789-09", "12345678909")]
    [InlineData("12.345.678/0001-99", "12345678000199")]
    public void Deve_normalizar_documento_removendo_caracteres(string input, string esperado)
    {
        var doc = new DocumentoCpfCnpj(input);
        Assert.Equal(esperado, doc.Valor);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("abc")]
    [InlineData("123")]
    public void Deve_lancar_quando_documento_invalido(string input)
        => Assert.Throws<ArgumentException>(() => new DocumentoCpfCnpj(input));
}
