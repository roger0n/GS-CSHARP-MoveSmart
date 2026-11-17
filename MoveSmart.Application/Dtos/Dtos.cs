using System.ComponentModel.DataAnnotations;
using MoveSmart.Domain.Enums;

namespace MoveSmart.Application.Dtos;

// ----- Usuário -----
public record UsuarioCriarDto
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string Nome { get; init; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; init; } = string.Empty;
}

public record UsuarioDto(int Id, string Nome, string Email, DateTime CriadoEm);

// ----- Linha -----
public record LinhaCriarDto
{
    [Required, StringLength(20)]
    public string Codigo { get; init; } = string.Empty;

    [Required, StringLength(100)]
    public string Origem { get; init; } = string.Empty;

    [Required, StringLength(100)]
    public string Destino { get; init; } = string.Empty;

    public TipoTransporte Tipo { get; init; } = TipoTransporte.Onibus;
}

public record LinhaDto(int Id, string Codigo, string Origem, string Destino, TipoTransporte Tipo);

public record LinhaRankingDto(int Id, string Codigo, string Origem, string Destino, int TotalFavoritos);

// ----- Favorito -----
public record FavoritoCriarDto
{
    [Range(1, int.MaxValue)]
    public int LinhaId { get; init; }
}

public record FavoritoDto(int LinhaId, string Codigo, string Origem, string Destino, DateTime CriadoEm);

// ----- Lotação -----
public record RelatoCriarDto
{
    [Range(1, int.MaxValue)]
    public int UsuarioId { get; init; }

    [Required]
    public NivelLotacao Nivel { get; init; }

    [StringLength(200)]
    public string? Observacao { get; init; }
}

public record RelatoLotacaoDto(int Id, int LinhaId, int UsuarioId, NivelLotacao Nivel, string? Observacao, DateTime DataRelato);

public record LotacaoResumoDto(
    int LinhaId,
    string CodigoLinha,
    NivelLotacao? NivelEstimado,
    int TotalRelatos,
    int JanelaMinutos,
    Dictionary<string, int> Distribuicao,
    string Fonte);
