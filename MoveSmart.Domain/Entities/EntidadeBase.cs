namespace MoveSmart.Domain.Entities;

/// <summary>Classe base de todas as entidades: identificador e data de criação.</summary>
public abstract class EntidadeBase
{
    public int Id { get; protected set; }
    public DateTime CriadoEm { get; protected set; } = DateTime.UtcNow;
}
