using Microsoft.EntityFrameworkCore;
using MoveSmart.Application.Interfaces;
using MoveSmart.Domain.Entities;
using MoveSmart.Infrastructure.Persistence;

namespace MoveSmart.Infrastructure.Repositories;

public class RelatoLotacaoRepository : IRelatoLotacaoRepository
{
    private readonly MoveSmartDbContext _ctx;

    public RelatoLotacaoRepository(MoveSmartDbContext ctx) => _ctx = ctx;

    public async Task<IReadOnlyList<RelatoLotacao>> ListarRecentesAsync(int linhaId, DateTime desdeUtc, CancellationToken ct = default) =>
        await _ctx.Relatos
            .AsNoTracking()
            .Where(r => r.LinhaId == linhaId && r.DataRelato >= desdeUtc)
            .OrderByDescending(r => r.DataRelato)
            .ToListAsync(ct);

    public async Task AdicionarAsync(RelatoLotacao relato, CancellationToken ct = default)
    {
        _ctx.Relatos.Add(relato);
        await _ctx.SaveChangesAsync(ct);
    }
}
