using Oficina.Domain.Cadastro.ValueObjects;
using Xunit;

namespace Oficina.Tests.Domain;

public class CadastroValueObjectsTests
{
    [Fact]
    public void DocumentoCpfCnpj_DeveNormalizarParaApenasDigitos()
    {
        var doc = DocumentoCpfCnpj.Criar("11.444.777/0001-61");
        Assert.Equal("11444777000161", doc.Valor);
    }

    [Fact]
    public void DocumentoCpfCnpj_Invalido_DeveFalhar()
    {
        Assert.Throws<ArgumentException>(() => DocumentoCpfCnpj.Criar("123"));
    }

    [Fact]
    public void Placa_DeveNormalizar()
    {
        var placa = Placa.Criar("abc-1234");
        Assert.Equal("ABC1234", placa.Valor);
    }

    [Fact]
    public void Renavam_DeveNormalizar()
    {
        var renavam = Renavam.Criar("12.345.678-901");
        Assert.Equal("12345678901", renavam.Valor);
    }
}
