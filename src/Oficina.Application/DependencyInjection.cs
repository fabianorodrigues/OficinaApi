using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Oficina.Application.UseCases.Cadastro;
using Oficina.Application.UseCases.CatalogoEstoque;
using Oficina.Application.UseCases.Oficina;

namespace Oficina.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        object value = services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // Cadastro
        services.AddScoped<CadastrarClienteUseCase>();
        services.AddScoped<AtualizarClienteUseCase>();
        services.AddScoped<CadastrarVeiculoUseCase>();
        services.AddScoped<AtualizarVeiculoUseCase>();
        services.AddScoped<ObterClienteUseCase>();
        services.AddScoped<ObterVeiculoUseCase>();
        services.AddScoped<ListarVeiculosPorClienteUseCase>();

        // Catálogo & Estoque
        services.AddScoped<CadastrarServicoUseCase>();
        services.AddScoped<CadastrarPecaUseCase>();
        services.AddScoped<ObterPecaUseCase>();
        services.AddScoped<CadastrarInsumoUseCase>();
        services.AddScoped<ObterInsumoUseCase>();
        services.AddScoped<ObterEstoquePecaUseCase>();
        services.AddScoped<ObterEstoqueInsumoUseCase>();
        services.AddScoped<AjustarEstoquePecaUseCase>();
        services.AddScoped<AjustarEstoqueInsumoUseCase>();

        // Oficina
        services.AddScoped<CriarOsPreventivaUseCase>();
        services.AddScoped<CriarOsCorretivaUseCase>();
        services.AddScoped<RegistrarDiagnosticoUseCase>();
        services.AddScoped<AprovarOrcamentoUseCase>();
        services.AddScoped<RecusarOrcamentoUseCase>();
        services.AddScoped<ObterOrdemServicoUseCase>();
        services.AddScoped<ListarOrdensServicoUseCase>();
        services.AddScoped<FinalizarOrdemServicoUseCase>();
        services.AddScoped<EntregarOrdemServicoUseCase>();
        services.AddScoped<ObterOrcamentoUseCase>();
        services.AddScoped<RelatorioTempoMedioExecucaoUseCase>();

        return services;
    }
}
