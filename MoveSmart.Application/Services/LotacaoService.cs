using MoveSmart.Application.Dtos;
using MoveSmart.Application.Interfaces;
using MoveSmart.Domain.Entities;
using MoveSmart.Domain.Exceptions;

namespace MoveSmart.Application.Services;

public class LotacaoService : ILotacaoService
{
    public const int JanelaMinutos = 60;
    private const string Fonte = "Relatos voluntários de passageiros (estimativa do MoveSmart, não é dado oficial da SPTrans)";

    private readonly IUsuarioRepository _usuarios;
    private readonly ILinhaRepository _linhas;
    private readonly IRelatoLotacaoRepository _relatos;
    private readonly ICalculadoraLotacao _calculadora;
    private readonly TimeProvider _relogio;

    public LotacaoService(
        IUsuarioRepository usuarios,
        ILinhaRepository linhas,
        IRelatoLotacaoRepository relatos,
        ICalculadoraLotacao calculadora,
        TimeProvider relogio)
    {
        _usuarios = usuarios;
        _linhas = linhas;
        _relatos = relatos;
        _calculadora = calculadora;
        _relogio = relogio;
    }

    public async Task<RelatoLotacaoDto> RegistrarRelatoAsync(int linhaId, RelatoCriarDto dto, CancellationToken ct = default)
    {
        if (await _linhas.ObterPorIdAsync(linhaId, ct) is null)
            throw new NaoEncontradoException(nameof(Linha), linhaId);

        if (await _usuarios.ObterPorIdAsync(dto.UsuarioId, ct) is null)
            throw new NaoEncontradoException(nameof(Usuario), dto.UsuarioId);

        var relato = new RelatoLotacao(dto.UsuarioId, linhaId, dto.Nivel, dto.Observacao, _relogio.GetUtcNow().UtcDateTime);
        await _relatos.AdicionarAsync(relato, ct);
        return relato.ToDto();
    }

    public async Task<LotacaoResumoDto> ObterResumoAsync(int linhaId, CancellationToken ct = default)
    {
        var linha = await _linhas.ObterPorIdAsync(linhaId, ct)
            ?? throw new NaoEncontradoException(nameof(Linha), linhaId);

        var agora = _relogio.GetUtcNow().UtcDateTime;
        var relatos = await _relatos.ListarRecentesAsync(linhaId, agora.AddMinutes(-JanelaMinutos), ct);

        var nivel = _calculadora.Calcular(relatos, agora);
        var distribuicao = relatos
            .GroupBy(r => r.Nivel)
            .ToDictionary(g => g.Key.ToString(), g => g.Count());

        return new LotacaoResumoDto(linha.Id, linha.Codigo, nivel, relatos.Count, JanelaMinutos, distribuicao, Fonte);
    }
}
