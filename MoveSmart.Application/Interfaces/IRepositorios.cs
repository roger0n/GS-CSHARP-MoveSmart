using MoveSmart.Domain.Entities;

namespace MoveSmart.Application.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct = default);
    Task AdicionarAsync(Usuario usuario, CancellationToken ct = default);
}

public interface ILinhaRepository
{
    Task<Linha?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<Linha?> ObterPorCodigoAsync(string codigo, CancellationToken ct = default);
    Task<IReadOnlyList<Linha>> ListarAsync(string? busca, CancellationToken ct = default);
    Task<IReadOnlyList<(Linha Linha, int TotalFavoritos)>> ListarMaisFavoritadasAsync(int top, CancellationToken ct = default);
    Task AdicionarAsync(Linha linha, CancellationToken ct = default);
}

public interface IFavoritoRepository
{
    Task<IReadOnlyList<Favorito>> ListarPorUsuarioAsync(int usuarioId, CancellationToken ct = default);
    Task<Favorito?> ObterAsync(int usuarioId, int linhaId, CancellationToken ct = default);
    Task AdicionarAsync(Favorito favorito, CancellationToken ct = default);
    Task RemoverAsync(Favorito favorito, CancellationToken ct = default);
}

public interface IRelatoLotacaoRepository
{
    Task<IReadOnlyList<RelatoLotacao>> ListarRecentesAsync(int linhaId, DateTime desdeUtc, CancellationToken ct = default);
    Task AdicionarAsync(RelatoLotacao relato, CancellationToken ct = default);
}
