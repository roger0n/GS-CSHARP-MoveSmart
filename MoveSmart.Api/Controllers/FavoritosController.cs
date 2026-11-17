using Microsoft.AspNetCore.Mvc;
using MoveSmart.Application.Dtos;
using MoveSmart.Application.Interfaces;

namespace MoveSmart.Api.Controllers;

[ApiController]
[Route("api/usuarios/{usuarioId:int}/favoritos")]
[Produces("application/json")]
public class FavoritosController : ControllerBase
{
    private readonly IFavoritoService _service;

    public FavoritosController(IFavoritoService service) => _service = service;

    /// <summary>Lista as linhas favoritas do usuário.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<FavoritoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<FavoritoDto>>> Listar(int usuarioId, CancellationToken ct) =>
        Ok(await _service.ListarAsync(usuarioId, ct));

    /// <summary>Adiciona uma linha aos favoritos do usuário.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(FavoritoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FavoritoDto>> Adicionar(int usuarioId, [FromBody] FavoritoCriarDto dto, CancellationToken ct)
    {
        var favorito = await _service.AdicionarAsync(usuarioId, dto.LinhaId, ct);
        return CreatedAtAction(nameof(Listar), new { usuarioId }, favorito);
    }

    /// <summary>Remove uma linha dos favoritos do usuário.</summary>
    [HttpDelete("{linhaId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int usuarioId, int linhaId, CancellationToken ct)
    {
        await _service.RemoverAsync(usuarioId, linhaId, ct);
        return NoContent();
    }
}
