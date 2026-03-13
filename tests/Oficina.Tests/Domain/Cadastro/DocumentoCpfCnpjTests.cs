using Oficina.Domain.Cadastro.ValueObjects;
using Xunit;

namespace Oficina.Tests.Domain.Cadastro;

public class DocumentoCpfCnpjTests
{
    [Theory]
    [InlineData("529.982.247-25", "52998224725")]
    [InlineData("04.252.011/0001-10", "04252011000110")]
    public void Deve_normalizar_documento_removendo_caracteres(string input, string esperado)
    {
        var doc = new DocumentoCpfCnpj(input);
        Assert.Equal(esperado, doc.Valor);
    }

    [Theory]
    [InlineData("123.456.789-09")]
    [InlineData("11111111111")]
    [InlineData("04.252.011/0001-11")]
    [InlineData("11.111.111/1111-11")]
    public void Deve_lancar_quando_digitos_verificadores_invalidos(string input)
        => Assert.Throws<ArgumentException>(() => new DocumentoCpfCnpj(input));

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("abc")]
    [InlineData("123")]
    public void Deve_lancar_quando_documento_invalido(string input)
        => Assert.Throws<ArgumentException>(() => new DocumentoCpfCnpj(input));
}
