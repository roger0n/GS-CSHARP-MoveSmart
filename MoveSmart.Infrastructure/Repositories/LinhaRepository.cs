using Microsoft.EntityFrameworkCore;
using MoveSmart.Application.Interfaces;
using MoveSmart.Domain.Entities;
using MoveSmart.Infrastructure.Persistence;

namespace MoveSmart.Infrastructure.Repositories;

public class LinhaRepository : ILinhaRepository
{
    private readonly MoveSmartDbContext _ctx;

    public LinhaRepository(MoveSmartDbContext ctx) => _ctx = ctx;

    public Task<Linha?> ObterPorIdAsync(int id, CancellationToken ct = default) =>
        _ctx.Linhas.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id && l.Ativa, ct);

    public Task<Linha?> ObterPorCodigoAsync(string codigo, CancellationToken ct = default) =>
        _ctx.Linhas.AsNoTracking().FirstOrDefaultAsync(l => l.Codigo == codigo, ct);

    public async Task<IReadOnlyList<Linha>> ListarAsync(string? busca, CancellationToken ct = default)
    {
        var query = _ctx.Linhas.AsNoTracking().Where(l => l.Ativa);

        if (!string.IsNullOrWhiteSpace(busca))
        {
            var termo = busca.Trim().ToUpper();
            query = query.Where(l =>
                l.Codigo.ToUpper().Contains(termo) ||
                l.Origem.ToUpper().Contains(termo) ||
                l.Destino.ToUpper().Contains(termo));
        }

        return await query.OrderBy(l => l.Codigo).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<(Linha Linha, int TotalFavoritos)>> ListarMaisFavoritadasAsync(int top, CancellationToken ct = default)
    {
        var resultado = await _ctx.Linhas
            .AsNoTracking()
            .Where(l => l.Ativa)
            .Select(l => new { Linha = l, Total = l.Favoritos.Count() })
            .OrderByDescending(x => x.Total)
            .ThenBy(x => x.Linha.Codigo)
            .Take(top)
            .ToListAsync(ct);

        return resultado.Select(x => (x.Linha, x.Total)).ToList();
    }

    public async Task AdicionarAsync(Linha linha, CancellationToken ct = default)
    {
        _ctx.Linhas.Add(linha);
        await _ctx.SaveChangesAsync(ct);
    }
}
