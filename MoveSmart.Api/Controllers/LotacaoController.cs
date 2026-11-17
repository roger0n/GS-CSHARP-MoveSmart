using Microsoft.AspNetCore.Mvc;
using MoveSmart.Application.Dtos;
using MoveSmart.Application.Interfaces;

namespace MoveSmart.Api.Controllers;

[ApiController]
[Route("api/linhas/{linhaId:int}")]
[Produces("application/json")]
public class LotacaoController : ControllerBase
{
    private readonly ILotacaoService _service;

    public LotacaoController(ILotacaoService service) => _service = service;

    /// <summary>Registra um relato de lotação feito por um passageiro.</summary>
    [HttpPost("relatos-lotacao")]
    [ProducesResponseType(typeof(RelatoLotacaoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RelatoLotacaoDto>> RegistrarRelato(int linhaId, [FromBody] RelatoCriarDto dto, CancellationToken ct)
    {
        var relato = await _service.RegistrarRelatoAsync(linhaId, dto, ct);
        return CreatedAtAction(nameof(ObterLotacao), new { linhaId }, relato);
    }

    /// <summary>Resumo da lotação estimada a partir dos relatos da última hora.</summary>
    [HttpGet("lotacao")]
    [ProducesResponseType(typeof(LotacaoResumoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LotacaoResumoDto>> ObterLotacao(int linhaId, CancellationToken ct) =>
        Ok(await _service.ObterResumoAsync(linhaId, ct));
}
