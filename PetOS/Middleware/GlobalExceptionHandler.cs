using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace PetOS.Middleware;

/// <summary>
/// Padroniza o tratamento de exceções da API.
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            ArgumentException =>
                (StatusCodes.Status400BadRequest,
                    "Parâmetros inválidos"),

            ValidationException =>
                (StatusCodes.Status400BadRequest,
                    "Dados inválidos"),

            KeyNotFoundException =>
                (StatusCodes.Status404NotFound,
                    "Recurso não encontrado"),

            _ =>
                (StatusCodes.Status500InternalServerError,
                    "Erro interno do servidor")
        };

        if (status >= 500)
        {
            _logger.LogError(
                exception,
                "Falha não tratada em {Path}",
                context.Request.Path);
        }
        else
        {
            _logger.LogWarning(
                exception,
                "Requisição inválida em {Path}",
                context.Request.Path);
        }

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = status >= 500
                ? "Ocorreu uma falha inesperada. " +
                  "Informe o identificador da requisição ao suporte."
                : exception.Message,
            Instance = context.Request.Path.ToString()
        };

        problem.Extensions["traceId"] =
            context.TraceIdentifier;

        problem.Extensions["correlationId"] =
            context.Response.Headers["X-Correlation-ID"].ToString();

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";

        await JsonSerializer.SerializeAsync(
            context.Response.Body,
            problem,
            new JsonSerializerOptions(JsonSerializerDefaults.Web),
            cancellationToken);

        return true;
    }
}