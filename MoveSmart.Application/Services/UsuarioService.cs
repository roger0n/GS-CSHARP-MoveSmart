using MoveSmart.Application.Dtos;
using MoveSmart.Application.Interfaces;
using MoveSmart.Domain.Entities;
using MoveSmart.Domain.Exceptions;

namespace MoveSmart.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarios;

    public UsuarioService(IUsuarioRepository usuarios) => _usuarios = usuarios;

    public async Task<UsuarioDto> CriarAsync(UsuarioCriarDto dto, CancellationToken ct = default)
    {
        var usuario = new Usuario(dto.Nome, dto.Email); // valida e normaliza o e-mail

        if (await _usuarios.ObterPorEmailAsync(usuario.Email, ct) is not null)
            throw new ConflitoException($"Já existe um usuário com o e-mail '{usuario.Email}'.");

        await _usuarios.AdicionarAsync(usuario, ct);
        return usuario.ToDto();
    }

    public async Task<UsuarioDto> ObterAsync(int id, CancellationToken ct = default)
    {
        var usuario = await _usuarios.ObterPorIdAsync(id, ct)
            ?? throw new NaoEncontradoException(nameof(Usuario), id);
        return usuario.ToDto();
    }
}
