using Microsoft.EntityFrameworkCore;
using MoveSmart.Domain.Entities;
using MoveSmart.Domain.Enums;

namespace MoveSmart.Infrastructure.Persistence;

public static class DbSeeder
{
    /// <summary>
    /// Carga inicial de linhas de exemplo.
    /// TODO: trocar pelos códigos/trajetos reais das linhas escolhidas pelo grupo (SPTrans).
    /// </summary>
    public static async Task SeedAsync(MoveSmartDbContext ctx, CancellationToken ct = default)
    {
        if (await ctx.Linhas.AnyAsync(ct)) return;

        ctx.Linhas.AddRange(
            new Linha("8000-10", "Terminal de exemplo A", "Terminal de exemplo B", TipoTransporte.Onibus),
            new Linha("2290-10", "Terminal de exemplo C", "Terminal de exemplo D", TipoTransporte.Onibus),
            new Linha("5110-10", "Terminal de exemplo E", "Terminal de exemplo F", TipoTransporte.Onibus));

        await ctx.SaveChangesAsync(ct);
    }
}
