using MoveSmart.Domain.Exceptions;

namespace MoveSmart.Domain.Entities;

public class Favorito : EntidadeBase
{
    public int UsuarioId { get; private set; }
    public int LinhaId { get; private set; }

    public Usuario Usuario { get; private set; } = null!;
    public Linha Linha { get; private set; } = null!;

    protected Favorito() { }

    public Favorito(int usuarioId, int linhaId)
    {
        if (usuarioId <= 0) throw new DominioException("Usuário inválido.");
        if (linhaId <= 0) throw new DominioException("Linha inválida.");
        UsuarioId = usuarioId;
        LinhaId = linhaId;
    }
}
