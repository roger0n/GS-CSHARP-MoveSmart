using MoveSmart.Domain.Entities;
using MoveSmart.Domain.Enums;
using MoveSmart.Domain.Exceptions;

namespace MoveSmart.Tests;

public class EntidadesTests
{
    [Fact]
    public void Usuario_Valido_NormalizaEmail()
    {
        var usuario = new Usuario("  Ana Souza ", "ANA@Email.com");
        Assert.Equal("Ana Souza", usuario.Nome);
        Assert.Equal("ana@email.com", usuario.Email);
    }

    [Theory]
    [InlineData("", "ana@email.com")]
    [InlineData("A", "ana@email.com")]
    [InlineData("Ana", "email-invalido")]
    [InlineData("Ana", "")]
    public void Usuario_Invalido_LancaDominioException(string nome, string email)
    {
        Assert.Throws<DominioException>(() => new Usuario(nome, email));
    }

    [Fact]
    public void Linha_NormalizaCodigoParaMaiusculo()
    {
        var linha = new Linha(" 273t-10 ", "Origem", "Destino", TipoTransporte.Onibus);
        Assert.Equal("273T-10", linha.Codigo);
        Assert.True(linha.Ativa);
    }

    [Fact]
    public void Linha_Desativar_MarcaComoInativa()
    {
        var linha = new Linha("8000-10", "A", "B", TipoTransporte.Onibus);
        linha.Desativar();
        Assert.False(linha.Ativa);
    }

    [Fact]
    public void Relato_ComNivelInvalido_LancaDominioException()
    {
        Assert.Throws<DominioException>(() => new RelatoLotacao(1, 1, (NivelLotacao)0, null));
    }

    [Fact]
    public void Relato_ComObservacaoMuitoLonga_LancaDominioException()
    {
        Assert.Throws<DominioException>(() => new RelatoLotacao(1, 1, NivelLotacao.Alta, new string('x', 201)));
    }
}
