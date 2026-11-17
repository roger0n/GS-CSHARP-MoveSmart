using Microsoft.EntityFrameworkCore;
using MoveSmart.Application.Interfaces;
using MoveSmart.Domain.Entities;
using MoveSmart.Infrastructure.Persistence;

namespace MoveSmart.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly MoveSmartDbContext _ctx;

    public UsuarioRepository(MoveSmartDbContext ctx) => _ctx = ctx;

    public Task<Usuario?> ObterPorIdAsync(int id, CancellationToken ct = default) =>
        _ctx.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct = default) =>
        _ctx.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task AdicionarAsync(Usuario usuario, CancellationToken ct = default)
    {
        _ctx.Usuarios.Add(usuario);
        await _ctx.SaveChangesAsync(ct);
    }
}
