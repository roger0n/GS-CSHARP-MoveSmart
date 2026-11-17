using Moq;
using MoveSmart.Application.Dtos;
using MoveSmart.Application.Interfaces;
using MoveSmart.Application.Services;
using MoveSmart.Domain.Entities;
using MoveSmart.Domain.Exceptions;

namespace MoveSmart.Tests;

public class UsuarioServiceTests
{
    private readonly Mock<IUsuarioRepository> _repo = new();

    [Fact]
    public async Task Criar_EmailNovo_PersisteUsuario()
    {
        _repo.Setup(r => r.ObterPorEmailAsync("ana@email.com", It.IsAny<CancellationToken>()))
             .ReturnsAsync((Usuario?)null);
        var service = new UsuarioService(_repo.Object);

        var resultado = await service.CriarAsync(new UsuarioCriarDto { Nome = "Ana", Email = "Ana@Email.com" });

        Assert.Equal("ana@email.com", resultado.Email);
        _repo.Verify(r => r.AdicionarAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Criar_EmailDuplicado_LancaConflito()
    {
        _repo.Setup(r => r.ObterPorEmailAsync("ana@email.com", It.IsAny<CancellationToken>()))
             .ReturnsAsync(new Usuario("Ana", "ana@email.com"));
        var service = new UsuarioService(_repo.Object);

        await Assert.ThrowsAsync<ConflitoException>(() =>
            service.CriarAsync(new UsuarioCriarDto { Nome = "Ana", Email = "ana@email.com" }));

        _repo.Verify(r => r.AdicionarAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Obter_Inexistente_LancaNaoEncontrado()
    {
        _repo.Setup(r => r.ObterPorIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Usuario?)null);
        var service = new UsuarioService(_repo.Object);

        await Assert.ThrowsAsync<NaoEncontradoException>(() => service.ObterAsync(99));
    }
}
