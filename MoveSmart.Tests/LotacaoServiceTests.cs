using Moq;
using MoveSmart.Application.Dtos;
using MoveSmart.Application.Interfaces;
using MoveSmart.Application.Services;
using MoveSmart.Domain.Entities;
using MoveSmart.Domain.Enums;
using MoveSmart.Domain.Exceptions;

namespace MoveSmart.Tests;

public class LotacaoServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarios = new();
    private readonly Mock<ILinhaRepository> _linhas = new();
    private readonly Mock<IRelatoLotacaoRepository> _relatos = new();

    private LotacaoService CriarService() => new(
        _usuarios.Object, _linhas.Object, _relatos.Object,
        new CalculadoraLotacaoPorMediaPonderada(), TimeProvider.System);

    private void LinhaExiste() =>
        _linhas.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
               .ReturnsAsync(new Linha("8000-10", "A", "B", TipoTransporte.Onibus));

    [Fact]
    public async Task Resumo_LinhaInexistente_LancaNaoEncontrado()
    {
        _linhas.Setup(r => r.ObterPorIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync((Linha?)null);

        await Assert.ThrowsAsync<NaoEncontradoException>(() => CriarService().ObterResumoAsync(5));
    }

    [Fact]
    public async Task Resumo_SemRelatos_NivelNuloETotalZero()
    {
        LinhaExiste();
        _relatos.Setup(r => r.ListarRecentesAsync(1, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RelatoLotacao>());

        var resumo = await CriarService().ObterResumoAsync(1);

        Assert.Null(resumo.NivelEstimado);
        Assert.Equal(0, resumo.TotalRelatos);
    }

    [Fact]
    public async Task Resumo_ComRelatosAltos_EstimaAltaEAgrupaDistribuicao()
    {
        LinhaExiste();
        _relatos.Setup(r => r.ListarRecentesAsync(1, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RelatoLotacao>
                {
                    new(1, 1, NivelLotacao.Alta, null),
                    new(2, 1, NivelLotacao.Alta, null),
                    new(3, 1, NivelLotacao.Media, null)
                });

        var resumo = await CriarService().ObterResumoAsync(1);

        Assert.Equal(NivelLotacao.Alta, resumo.NivelEstimado);
        Assert.Equal(3, resumo.TotalRelatos);
        Assert.Equal(2, resumo.Distribuicao["Alta"]);
        Assert.Equal(1, resumo.Distribuicao["Media"]);
    }

    [Fact]
    public async Task Registrar_UsuarioInexistente_LancaNaoEncontrado()
    {
        LinhaExiste();
        _usuarios.Setup(r => r.ObterPorIdAsync(9, It.IsAny<CancellationToken>())).ReturnsAsync((Usuario?)null);

        var dto = new RelatoCriarDto { UsuarioId = 9, Nivel = NivelLotacao.Alta };

        await Assert.ThrowsAsync<NaoEncontradoException>(() => CriarService().RegistrarRelatoAsync(1, dto));
    }

    [Fact]
    public async Task Registrar_Valido_PersisteRelato()
    {
        LinhaExiste();
        _usuarios.Setup(r => r.ObterPorIdAsync(2, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new Usuario("Ana", "ana@email.com"));

        var dto = new RelatoCriarDto { UsuarioId = 2, Nivel = NivelLotacao.Media, Observacao = "Ônibus cheio na porta" };
        var resultado = await CriarService().RegistrarRelatoAsync(1, dto);

        Assert.Equal(NivelLotacao.Media, resultado.Nivel);
        _relatos.Verify(r => r.AdicionarAsync(It.IsAny<RelatoLotacao>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
