using MoveSmart.Domain.Entities;

namespace MoveSmart.Application.Dtos;

public static class Mapeamentos
{
    public static UsuarioDto ToDto(this Usuario u) => new(u.Id, u.Nome, u.Email, u.CriadoEm);

    public static LinhaDto ToDto(this Linha l) => new(l.Id, l.Codigo, l.Origem, l.Destino, l.Tipo);

    public static FavoritoDto ToDto(this Favorito f) =>
        new(f.LinhaId, f.Linha.Codigo, f.Linha.Origem, f.Linha.Destino, f.CriadoEm);

    public static RelatoLotacaoDto ToDto(this RelatoLotacao r) =>
        new(r.Id, r.LinhaId, r.UsuarioId, r.Nivel, r.Observacao, r.DataRelato);
}
