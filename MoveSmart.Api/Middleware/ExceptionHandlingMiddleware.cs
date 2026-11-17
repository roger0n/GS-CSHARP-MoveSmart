using Microsoft.AspNetCore.Mvc;
using MoveSmart.Domain.Exceptions;

namespace MoveSmart.Api.Middleware;

/// <summary>Converte exceções de domínio em respostas ProblemDetails com o status HTTP adequado.</summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // Cliente cancelou a requisição; nada a responder.
        }
        catch (Exception ex)
        {
            var (status, titulo) = ex switch
            {
                NaoEncontradoException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
                ConflitoException => (StatusCodes.Status409Conflict, "Conflito"),
                DominioException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
                _ => (StatusCodes.Status500InternalServerError, "Erro interno")
            };

            if (status == StatusCodes.Status500InternalServerError)
                _logger.LogError(ex, "Erro não tratado");

            var problema = new ProblemDetails
            {
                Status = status,
                Title = titulo,
                Detail = status == StatusCodes.Status500InternalServerError
                    ? "Ocorreu um erro inesperado."
                    : ex.Message
            };

            context.Response.StatusCode = status;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problema);
        }
    }
}
