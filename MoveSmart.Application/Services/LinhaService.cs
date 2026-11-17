using MoveSmart.Application.Dtos;
using MoveSmart.Application.Interfaces;
using MoveSmart.Domain.Entities;
using MoveSmart.Domain.Exceptions;

namespace MoveSmart.Application.Services;

public class LinhaService : ILinhaService
{
    private readonly ILinhaRepository _linhas;

    public LinhaService(ILinhaRepository linhas) => _linhas = linhas;

    public async Task<IReadOnlyList<LinhaDto>> ListarAsync(string? busca, CancellationToken ct = default)
    {
        var linhas = await _linhas.ListarAsync(busca, ct);
        return linhas.Select(l => l.ToDto()).ToList();
    }

    public async Task<LinhaDto> ObterAsync(int id, CancellationToken ct = default)
    {
        var linha = await _linhas.ObterPorIdAsync(id, ct)
            ?? throw new NaoEncontradoException(nameof(Linha), id);
        return linha.ToDto();
    }

    public async Task<LinhaDto> CriarAsync(LinhaCriarDto dto, CancellationToken ct = default)
    {
        var linha = new Linha(dto.Codigo, dto.Origem, dto.Destino, dto.Tipo);

        if (await _linhas.ObterPorCodigoAsync(linha.Codigo, ct) is not null)
            throw new ConflitoException($"Já existe uma linha com o código '{linha.Codigo}'.");

        await _linhas.AdicionarAsync(linha, ct);
        return linha.ToDto();
    }

    public async Task<IReadOnlyList<LinhaRankingDto>> MaisFavoritadasAsync(int top, CancellationToken ct = default)
    {
        top = Math.Clamp(top, 1, 20);
        var ranking = await _linhas.ListarMaisFavoritadasAsync(top, ct);
        return ranking
            .Select(x => new LinhaRankingDto(x.Linha.Id, x.Linha.Codigo, x.Linha.Origem, x.Linha.Destino, x.TotalFavoritos))
            .ToList();
    }
}
