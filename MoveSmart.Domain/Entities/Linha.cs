using MoveSmart.Domain.Enums;
using MoveSmart.Domain.Exceptions;

namespace MoveSmart.Domain.Entities;

public class Linha : EntidadeBase
{
    private readonly List<Favorito> _favoritos = new();
    private readonly List<RelatoLotacao> _relatos = new();

    public string Codigo { get; private set; } = string.Empty;
    public string Origem { get; private set; } = string.Empty;
    public string Destino { get; private set; } = string.Empty;
    public TipoTransporte Tipo { get; private set; }
    public bool Ativa { get; private set; } = true;

    public IReadOnlyCollection<Favorito> Favoritos => _favoritos;
    public IReadOnlyCollection<RelatoLotacao> Relatos => _relatos;

    protected Linha() { }

    public Linha(string codigo, string origem, string destino, TipoTransporte tipo)
    {
        Codigo = Validar(codigo, "código", 20).ToUpperInvariant();
        Origem = Validar(origem, "origem", 100);
        Destino = Validar(destino, "destino", 100);

        if (!Enum.IsDefined(tipo))
            throw new DominioException("Tipo de transporte inválido.");
        Tipo = tipo;
    }

    public void Desativar() => Ativa = false;

    private static string Validar(string? valor, string campo, int max)
    {
        var texto = valor?.Trim();
        if (string.IsNullOrEmpty(texto))
            throw new DominioException($"O campo {campo} é obrigatório.");
        if (texto.Length > max)
            throw new DominioException($"O campo {campo} deve ter no máximo {max} caracteres.");
        return texto;
    }
}
