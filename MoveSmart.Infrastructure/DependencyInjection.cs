using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MoveSmart.Application.Interfaces;
using MoveSmart.Infrastructure.Persistence;
using MoveSmart.Infrastructure.Repositories;

namespace MoveSmart.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<MoveSmartDbContext>(o => o.UseSqlite(connectionString));

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ILinhaRepository, LinhaRepository>();
        services.AddScoped<IFavoritoRepository, FavoritoRepository>();
        services.AddScoped<IRelatoLotacaoRepository, RelatoLotacaoRepository>();
        return services;
    }

    /// <summary>Aplica as migrations (ou cria o banco, se ainda não houver migrations) e carrega os dados iniciais.</summary>
    public static async Task InicializarBancoAsync(this IServiceProvider provider)
    {
        using var scope = provider.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<MoveSmartDbContext>();

        if (ctx.Database.GetMigrations().Any())
            await ctx.Database.MigrateAsync();
        else
            await ctx.Database.EnsureCreatedAsync();

        await DbSeeder.SeedAsync(ctx);
    }
}
