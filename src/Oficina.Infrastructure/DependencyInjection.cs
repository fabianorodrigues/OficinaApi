using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oficina.Application.Abstractions.Email;
using Oficina.Application.Abstractions.Notificacoes;
using Oficina.Application.Abstractions.Repositorios;
using Oficina.Infrastructure.Email.Configurations;
using Oficina.Infrastructure.Email.Providers;
using Oficina.Infrastructure.Notificacoes;
using Oficina.Infrastructure.Persistencia;
using Oficina.Infrastructure.Repositorios;

namespace Oficina.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("SqlServer");
        Console.WriteLine($"ConnectionString usada: {cs}");
        services.AddDbContext<OficinaDbContext>(opt => opt.UseSqlServer(cs));

        services.Configure<EmailSettings>(config.GetSection("EmailSettings"));

        services.AddScoped<ICadastroRepository, CadastroRepository>();
        services.AddScoped<ICatalogoEstoqueRepository, CatalogoEstoqueRepository>();
        services.AddScoped<IOficinaRepository, OficinaRepository>();
        services.AddScoped<IEmailSender, MailKitEmailSender>();

        services.AddScoped<INotificadorCliente, NotificadorCliente>();

        return services;
    }
}
