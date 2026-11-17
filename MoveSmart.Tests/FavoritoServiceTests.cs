using Moq;
using MoveSmart.Application.Interfaces;
using MoveSmart.Application.Services;
using MoveSmart.Domain.Entities;
using MoveSmart.Domain.Enums;
using MoveSmart.Domain.Exceptions;

namespace MoveSmart.Tests;

public class FavoritoServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarios = new();
    private readonly Mock<ILinhaRepository> _linhas = new();
    private readonly Mock<IFavoritoRepository> _favoritos = new();

    private FavoritoService CriarService() => new(_usuarios.Object, _linhas.Object, _favoritos.Object);

    private void UsuarioELinhaExistem()
    {
        _usuarios.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new Usuario("Ana", "ana@email.com"));
        _linhas.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
               .ReturnsAsync(new Linha("8000-10", "A", "B", TipoTransporte.Onibus));
    }

    [Fact]
    public async Task Adicionar_UsuarioInexistente_LancaNaoEncontrado()
    {
        _usuarios.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Usuario?)null);

        await Assert.ThrowsAsync<NaoEncontradoException>(() => CriarService().AdicionarAsync(1, 1));
    }

    [Fact]
    public async Task Adicionar_LinhaInexistente_LancaNaoEncontrado()
    {
        _usuarios.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new Usuario("Ana", "ana@email.com"));
        _linhas.Setup(r => r.ObterPorIdAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync((Linha?)null);

        await Assert.ThrowsAsync<NaoEncontradoException>(() => CriarService().AdicionarAsync(1, 7));
    }

    [Fact]
    public async Task Adicionar_Duplicado_LancaConflito()
    {
        UsuarioELinhaExistem();
        _favoritos.Setup(r => r.ObterAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(new Favorito(1, 1));

        await Assert.ThrowsAsync<ConflitoException>(() => CriarService().AdicionarAsync(1, 1));
    }

    [Fact]
    public async Task Adicionar_Valido_PersisteFavorito()
    {
        UsuarioELinhaExistem();
        _favoritos.Setup(r => r.ObterAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync((Favorito?)null);

        var resultado = await CriarService().AdicionarAsync(1, 1);

        Assert.Equal("8000-10", resultado.Codigo);
        _favoritos.Verify(r => r.AdicionarAsync(It.IsAny<Favorito>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Remover_FavoritoInexistente_LancaNaoEncontrado()
    {
        UsuarioELinhaExistem();
        _favoritos.Setup(r => r.ObterAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync((Favorito?)null);

        await Assert.ThrowsAsync<NaoEncontradoException>(() => CriarService().RemoverAsync(1, 1));
    }
}
