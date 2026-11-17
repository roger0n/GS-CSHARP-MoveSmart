using MoveSmart.Domain.Enums;
using MoveSmart.Domain.Exceptions;

namespace MoveSmart.Domain.Entities;

/// <summary>Relato voluntário de um passageiro sobre a lotação de uma linha.</summary>
public class RelatoLotacao : EntidadeBase
{
    public int UsuarioId { get; private set; }
    public int LinhaId { get; private set; }
    public NivelLotacao Nivel { get; private set; }
    public string? Observacao { get; private set; }
    public DateTime DataRelato { get; private set; }

    public Usuario Usuario { get; private set; } = null!;
    public Linha Linha { get; private set; } = null!;

    protected RelatoLotacao() { }

    public RelatoLotacao(int usuarioId, int linhaId, NivelLotacao nivel, string? observacao)
        : this(usuarioId, linhaId, nivel, observacao, DateTime.UtcNow) { }

    public RelatoLotacao(int usuarioId, int linhaId, NivelLotacao nivel, string? observacao, DateTime dataRelatoUtc)
    {
        if (usuarioId <= 0) throw new DominioException("Usuário inválido.");
        if (linhaId <= 0) throw new DominioException("Linha inválida.");
        if (!Enum.IsDefined(nivel)) throw new DominioException("Nível de lotação inválido.");

        var obs = observacao?.Trim();
        if (obs is { Length: > 200 })
            throw new DominioException("A observação deve ter no máximo 200 caracteres.");

        UsuarioId = usuarioId;
        LinhaId = linhaId;
        Nivel = nivel;
        Observacao = string.IsNullOrEmpty(obs) ? null : obs;
        DataRelato = dataRelatoUtc;
    }
}
