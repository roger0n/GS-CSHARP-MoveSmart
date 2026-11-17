using Microsoft.EntityFrameworkCore;
using MoveSmart.Application.Interfaces;
using MoveSmart.Domain.Entities;
using MoveSmart.Infrastructure.Persistence;

namespace MoveSmart.Infrastructure.Repositories;

public class FavoritoRepository : IFavoritoRepository
{
    private readonly MoveSmartDbContext _ctx;

    public FavoritoRepository(MoveSmartDbContext ctx) => _ctx = ctx;

    public async Task<IReadOnlyList<Favorito>> ListarPorUsuarioAsync(int usuarioId, CancellationToken ct = default) =>
        await _ctx.Favoritos
            .AsNoTracking()
            .Include(f => f.Linha)
            .Where(f => f.UsuarioId == usuarioId)
            .OrderBy(f => f.Linha.Codigo)
            .ToListAsync(ct);

    public Task<Favorito?> ObterAsync(int usuarioId, int linhaId, CancellationToken ct = default) =>
        _ctx.Favoritos.FirstOrDefaultAsync(f => f.UsuarioId == usuarioId && f.LinhaId == linhaId, ct);

    public async Task AdicionarAsync(Favorito favorito, CancellationToken ct = default)
    {
        _ctx.Favoritos.Add(favorito);
        await _ctx.SaveChangesAsync(ct);
    }

    public async Task RemoverAsync(Favorito favorito, CancellationToken ct = default)
    {
        _ctx.Favoritos.Remove(favorito);
        await _ctx.SaveChangesAsync(ct);
    }
}
