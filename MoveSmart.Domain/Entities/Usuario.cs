using System.Net.Mail;
using MoveSmart.Domain.Exceptions;

namespace MoveSmart.Domain.Entities;

public class Usuario : EntidadeBase
{
    private readonly List<Favorito> _favoritos = new();
    private readonly List<RelatoLotacao> _relatos = new();

    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    public IReadOnlyCollection<Favorito> Favoritos => _favoritos;
    public IReadOnlyCollection<RelatoLotacao> Relatos => _relatos;

    protected Usuario() { } // exigido pelo EF Core

    public Usuario(string nome, string email)
    {
        Nome = ValidarNome(nome);
        Email = ValidarEmail(email);
    }

    private static string ValidarNome(string? nome)
    {
        var valor = nome?.Trim();
        if (string.IsNullOrEmpty(valor) || valor.Length < 2)
            throw new DominioException("O nome deve ter ao menos 2 caracteres.");
        if (valor.Length > 100)
            throw new DominioException("O nome deve ter no máximo 100 caracteres.");
        return valor;
    }

    private static string ValidarEmail(string? email)
    {
        var valor = email?.Trim();
        if (string.IsNullOrEmpty(valor) || valor.Length > 150)
            throw new DominioException("E-mail inválido.");
        try
        {
            var endereco = new MailAddress(valor);
            if (endereco.Address != valor) throw new FormatException();
            return endereco.Address.ToLowerInvariant();
        }
        catch (FormatException)
        {
            throw new DominioException("E-mail inválido.");
        }
    }
}
