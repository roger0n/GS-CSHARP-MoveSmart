using Microsoft.Extensions.DependencyInjection;
using MoveSmart.Application.Interfaces;
using MoveSmart.Application.Services;

namespace MoveSmart.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<ICalculadoraLotacao, CalculadoraLotacaoPorMediaPonderada>();

        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<ILinhaService, LinhaService>();
        services.AddScoped<IFavoritoService, FavoritoService>();
        services.AddScoped<ILotacaoService, LotacaoService>();
        return services;
    }
}
