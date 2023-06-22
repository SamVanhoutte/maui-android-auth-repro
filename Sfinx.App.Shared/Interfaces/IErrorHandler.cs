using MudBlazor;
using Sfinx.Backend.WebAPI.Client;
using Exception = System.Exception;

namespace Sfinx.App.Shared.Interfaces;

public interface IErrorHandler
{
    Task ProcessError(ApiException<ProblemDetails> e, string? errorMessage = null, Severity severity = Severity.Error);
    Task ProcessError(Exception e, Severity severity = Severity.Error);
    Task<ApiResult<T>> ExecuteAndHandleExceptionsAsync<T>(Func<Task<T>> apiTask, bool ignoreExceptions = false, IEnumerable<int>? statusCodesToIgnore = null);
    Task<ApiResult<bool>> ExecuteAndHandleExceptionsAsync(Func<Task> apiTask, bool ignoreExceptions = false, IEnumerable<int>? statusCodesToIgnore = null);
}