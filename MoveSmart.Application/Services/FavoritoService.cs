using MoveSmart.Application.Dtos;
using MoveSmart.Application.Interfaces;
using MoveSmart.Domain.Entities;
using MoveSmart.Domain.Exceptions;

namespace MoveSmart.Application.Services;

public class FavoritoService : IFavoritoService
{
    private readonly IUsuarioRepository _usuarios;
    private readonly ILinhaRepository _linhas;
    private readonly IFavoritoRepository _favoritos;

    public FavoritoService(IUsuarioRepository usuarios, ILinhaRepository linhas, IFavoritoRepository favoritos)
    {
        _usuarios = usuarios;
        _linhas = linhas;
        _favoritos = favoritos;
    }

    public async Task<IReadOnlyList<FavoritoDto>> ListarAsync(int usuarioId, CancellationToken ct = default)
    {
        await GarantirUsuarioAsync(usuarioId, ct);
        var favoritos = await _favoritos.ListarPorUsuarioAsync(usuarioId, ct);
        return favoritos.Select(f => f.ToDto()).ToList();
    }

    public async Task<FavoritoDto> AdicionarAsync(int usuarioId, int linhaId, CancellationToken ct = default)
    {
        await GarantirUsuarioAsync(usuarioId, ct);

        var linha = await _linhas.ObterPorIdAsync(linhaId, ct)
            ?? throw new NaoEncontradoException(nameof(Linha), linhaId);

        if (await _favoritos.ObterAsync(usuarioId, linhaId, ct) is not null)
            throw new ConflitoException("Esta linha já está nos favoritos do usuário.");

        var favorito = new Favorito(usuarioId, linhaId);
        await _favoritos.AdicionarAsync(favorito, ct);

        return new FavoritoDto(linha.Id, linha.Codigo, linha.Origem, linha.Destino, favorito.CriadoEm);
    }

    public async Task RemoverAsync(int usuarioId, int linhaId, CancellationToken ct = default)
    {
        await GarantirUsuarioAsync(usuarioId, ct);

        var favorito = await _favoritos.ObterAsync(usuarioId, linhaId, ct)
            ?? throw new NaoEncontradoException("Favorito", $"{usuarioId}/{linhaId}");

        await _favoritos.RemoverAsync(favorito, ct);
    }

    private async Task GarantirUsuarioAsync(int usuarioId, CancellationToken ct)
    {
        if (await _usuarios.ObterPorIdAsync(usuarioId, ct) is null)
            throw new NaoEncontradoException(nameof(Usuario), usuarioId);
    }
}
