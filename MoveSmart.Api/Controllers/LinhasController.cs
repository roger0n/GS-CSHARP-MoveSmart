using Microsoft.AspNetCore.Mvc;
using MoveSmart.Application.Dtos;
using MoveSmart.Application.Interfaces;

namespace MoveSmart.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class LinhasController : ControllerBase
{
    private readonly ILinhaService _service;

    public LinhasController(ILinhaService service) => _service = service;

    /// <summary>Lista as linhas, com busca opcional por código, origem ou destino.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<LinhaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<LinhaDto>>> Listar([FromQuery] string? busca, CancellationToken ct) =>
        Ok(await _service.ListarAsync(busca, ct));

    /// <summary>Consulta uma linha pelo id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(LinhaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LinhaDto>> Obter(int id, CancellationToken ct) =>
        Ok(await _service.ObterAsync(id, ct));

    /// <summary>Ranking das linhas mais favoritadas pelos usuários.</summary>
    [HttpGet("mais-favoritadas")]
    [ProducesResponseType(typeof(IReadOnlyList<LinhaRankingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<LinhaRankingDto>>> MaisFavoritadas([FromQuery] int top = 5, CancellationToken ct = default) =>
        Ok(await _service.MaisFavoritadasAsync(top, ct));

    /// <summary>Cadastra uma nova linha.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(LinhaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LinhaDto>> Criar([FromBody] LinhaCriarDto dto, CancellationToken ct)
    {
        var criada = await _service.CriarAsync(dto, ct);
        return CreatedAtAction(nameof(Obter), new { id = criada.Id }, criada);
    }
}
