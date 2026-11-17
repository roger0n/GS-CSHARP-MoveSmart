using MoveSmart.Application.Interfaces;
using MoveSmart.Domain.Entities;
using MoveSmart.Domain.Enums;

namespace MoveSmart.Application.Services;

/// <summary>
/// Média ponderada dos relatos: quanto mais recente o relato, maior o peso
/// (peso = 1 / (1 + minutosAtrás / 15)).
/// </summary>
public class CalculadoraLotacaoPorMediaPonderada : ICalculadoraLotacao
{
    private const double MinutosMeiaVida = 15.0;

    public NivelLotacao? Calcular(IEnumerable<RelatoLotacao> relatos, DateTime agoraUtc)
    {
        var pesados = relatos
            .Select(r => new
            {
                Valor = (double)(int)r.Nivel,
                Peso = 1.0 / (1.0 + Math.Max(0, (agoraUtc - r.DataRelato).TotalMinutes) / MinutosMeiaVida)
            })
            .ToList();

        if (pesados.Count == 0) return null;

        var media = pesados.Sum(p => p.Valor * p.Peso) / pesados.Sum(p => p.Peso);

        return media < 1.67 ? NivelLotacao.Baixa
             : media < 2.34 ? NivelLotacao.Media
             : NivelLotacao.Alta;
    }
}
