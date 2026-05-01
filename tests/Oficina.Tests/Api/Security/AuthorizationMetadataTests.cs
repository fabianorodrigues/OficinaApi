using Microsoft.AspNetCore.Authorization;
using Oficina.Api.Controllers;
using Oficina.Api.Security;
using Xunit;

namespace Oficina.Tests.Api.Security;

public class AuthorizationMetadataTests
{
    [Fact]
    public void AuthController_DeveSerAnonimo()
    {
        Assert.NotNull(Attribute.GetCustomAttribute(typeof(AuthController), typeof(AllowAnonymousAttribute)));
    }

    [Fact]
    public void AdminFuncionariosController_DeveSerAdminOnly()
    {
        var attr = Assert.IsType<AuthorizeAttribute>(
            Attribute.GetCustomAttribute(typeof(AdminFuncionariosController), typeof(AuthorizeAttribute)));

        Assert.Equal(Policies.AdminOnly, attr.Policy);
    }

    [Fact]
    public void MinhasOrdensServicoController_DeveSerClienteOnly()
    {
        var attr = Assert.IsType<AuthorizeAttribute>(
            Attribute.GetCustomAttribute(typeof(MinhasOrdensServicoController), typeof(AuthorizeAttribute)));

        Assert.Equal(Policies.ClienteOnly, attr.Policy);
    }

    [Fact]
    public void OrcamentosAprovarRecusar_DeveSerAdminOnly()
    {
        var aprovar = typeof(OrcamentosController).GetMethod(nameof(OrcamentosController.Aprovar))!;
        var recusar = typeof(OrcamentosController).GetMethod(nameof(OrcamentosController.Recusar))!;

        Assert.Equal(Policies.AdminOnly, aprovar.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true).Cast<AuthorizeAttribute>().Single().Policy);
        Assert.Equal(Policies.AdminOnly, recusar.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true).Cast<AuthorizeAttribute>().Single().Policy);
    }
}
