using MoveSmart.Application.Services;
using MoveSmart.Domain.Entities;
using MoveSmart.Domain.Enums;

namespace MoveSmart.Tests;

public class CalculadoraLotacaoTests
{
    private static readonly DateTime Agora = new(2026, 11, 15, 12, 0, 0, DateTimeKind.Utc);
    private readonly CalculadoraLotacaoPorMediaPonderada _calculadora = new();

    private static RelatoLotacao Relato(NivelLotacao nivel, int minutosAtras) =>
        new(1, 1, nivel, null, Agora.AddMinutes(-minutosAtras));

    [Fact]
    public void SemRelatos_RetornaNulo()
    {
        Assert.Null(_calculadora.Calcular(Array.Empty<RelatoLotacao>(), Agora));
    }

    [Theory]
    [InlineData(NivelLotacao.Baixa)]
    [InlineData(NivelLotacao.Media)]
    [InlineData(NivelLotacao.Alta)]
    public void RelatoUnico_RetornaMesmoNivel(NivelLotacao nivel)
    {
        var resultado = _calculadora.Calcular(new[] { Relato(nivel, 5) }, Agora);
        Assert.Equal(nivel, resultado);
    }

    [Fact]
    public void RelatosIguaisEmTempoIgual_BaixaEAlta_ResultaEmMedia()
    {
        var relatos = new[] { Relato(NivelLotacao.Baixa, 10), Relato(NivelLotacao.Alta, 10) };
        Assert.Equal(NivelLotacao.Media, _calculadora.Calcular(relatos, Agora));
    }

    [Fact]
    public void RelatoRecenteTemMaisPesoQueRelatoAntigo()
    {
        var relatos = new[] { Relato(NivelLotacao.Alta, 0), Relato(NivelLotacao.Baixa, 50) };
        Assert.Equal(NivelLotacao.Alta, _calculadora.Calcular(relatos, Agora));
    }
}
