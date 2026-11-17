using MoveSmart.Application.Dtos;
using MoveSmart.Domain.Entities;
using MoveSmart.Domain.Enums;

namespace MoveSmart.Application.Interfaces;

public interface IUsuarioService
{
    Task<UsuarioDto> CriarAsync(UsuarioCriarDto dto, CancellationToken ct = default);
    Task<UsuarioDto> ObterAsync(int id, CancellationToken ct = default);
}

public interface ILinhaService
{
    Task<IReadOnlyList<LinhaDto>> ListarAsync(string? busca, CancellationToken ct = default);
    Task<LinhaDto> ObterAsync(int id, CancellationToken ct = default);
    Task<LinhaDto> CriarAsync(LinhaCriarDto dto, CancellationToken ct = default);
    Task<IReadOnlyList<LinhaRankingDto>> MaisFavoritadasAsync(int top, CancellationToken ct = default);
}

public interface IFavoritoService
{
    Task<IReadOnlyList<FavoritoDto>> ListarAsync(int usuarioId, CancellationToken ct = default);
    Task<FavoritoDto> AdicionarAsync(int usuarioId, int linhaId, CancellationToken ct = default);
    Task RemoverAsync(int usuarioId, int linhaId, CancellationToken ct = default);
}

public interface ILotacaoService
{
    Task<RelatoLotacaoDto> RegistrarRelatoAsync(int linhaId, RelatoCriarDto dto, CancellationToken ct = default);
    Task<LotacaoResumoDto> ObterResumoAsync(int linhaId, CancellationToken ct = default);
}

/// <summary>Estratégia de cálculo do nível de lotação a partir de relatos (permite trocar o algoritmo).</summary>
public interface ICalculadoraLotacao
{
    NivelLotacao? Calcular(IEnumerable<RelatoLotacao> relatos, DateTime agoraUtc);
}
