using LocadoraFilmes.Api.Exceptions;
using LocadoraFilmes.Validation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace LocadoraFilmes.Api.Handlers;

/// <summary>Converte exceções de negócio em respostas HTTP (ProblemDetails) adequadas.</summary>
public sealed class ApiExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ApiExceptionHandler> _logger;

    public ApiExceptionHandler(ILogger<ApiExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        IResult resultado;

        switch (exception)
        {
            case ValidacaoException validacao:
                resultado = Results.ValidationProblem(
                    validacao.Resultado.ParaDicionario(),
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Um ou mais erros de validação ocorreram.");
                break;

            case RecursoNaoEncontradoException:
                resultado = Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Recurso não encontrado.",
                    detail: exception.Message);
                break;

            case ConflitoException:
                resultado = Results.Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Conflito.",
                    detail: exception.Message);
                break;

            case RegraNegocioException:
                resultado = Results.Problem(
                    statusCode: StatusCodes.Status422UnprocessableEntity,
                    title: "Regra de negócio violada.",
                    detail: exception.Message);
                break;

            case CredenciaisInvalidasException:
                resultado = Results.Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Não autenticado.",
                    detail: exception.Message);
                break;

            case DbUpdateException:
                _logger.LogWarning(exception, "Violação de integridade ao salvar dados.");
                resultado = Results.Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Conflito.",
                    detail: "A operação viola uma restrição de integridade dos dados (duplicidade ou registro em uso).");
                break;

            default:
                _logger.LogError(exception, "Erro não tratado.");
                resultado = Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Erro interno do servidor.",
                    detail: "Ocorreu um erro inesperado. Tente novamente mais tarde.");
                break;
        }

        await resultado.ExecuteAsync(httpContext);
        return true;
    }
}
