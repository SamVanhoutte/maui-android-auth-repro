using System.Collections;
using MudBlazor;
using Sfinx.App.Shared.Interfaces;
using Sfinx.Backend.WebAPI.Client;
using Exception = System.Exception;

namespace Sfinx.App.Shared.Services.ErrorHandling;

public class ApiErrorHandler : IErrorHandler
{
    private readonly IAlertNotifier alertNotifier;
    private readonly ICrashReportHandler crashReportHandler;

    public ApiErrorHandler(IAlertNotifier alertNotifier, ICrashReportHandler crashReportHandler)
    {
        this.alertNotifier = alertNotifier;
        this.crashReportHandler = crashReportHandler;
    }

    public async Task ProcessError(ApiException<ProblemDetails> e, string? displayMessage = null,
        Severity severity = Severity.Error)
    {
        await crashReportHandler.HandleExceptionAsync(e);
        var errorMessage = $"{e.Message}: {e.Result.Detail}";
        if (!string.IsNullOrEmpty(displayMessage))
        {
            errorMessage = errorMessage + "\r\n" + errorMessage;
        }
        await NotifyAsync(errorMessage, severity == Severity.Error);
    }

    public async Task ProcessError(Exception e, Severity severity = Severity.Error)
    {
        await crashReportHandler.HandleExceptionAsync(e);
        await NotifyAsync(e.Message, severity == Severity.Error);
    }

    public async Task<ApiResult<T>> ExecuteAndHandleExceptionsAsync<T>(Func<Task<T>> apiTask,
        bool ignoreExceptions = false, IEnumerable<int>? statusCodesToIgnore = null)
    {
        var success = false;
        int httpStatus = -1;
        string? errorMessage = null;
        T? response = default;
        try
        {
            response = await apiTask();
            success = true;
        }
        catch (ApiException<ProblemDetails> e)
        {
            httpStatus = e.StatusCode;
            if (!ShouldIgnore(ignoreExceptions, statusCodesToIgnore, httpStatus))
            {
                await ProcessError(e);
            }
        }
        catch (ApiException e)
        {
            httpStatus = e.StatusCode;
            if (!ShouldIgnore(ignoreExceptions, statusCodesToIgnore, httpStatus))
            {
                await ProcessError(e);
            }
        }
        catch (Exception e)
        {
            if (!ignoreExceptions)
            {
                await ProcessError(e);
            }
        }

        return new ApiResult<T> {Success = success, Response = response, HttpStatus = httpStatus};
    }

    private bool ShouldIgnore(bool ignoreExceptions, IEnumerable<int>? statusCodesToIgnore, int httpStatus)
    {
        return ignoreExceptions || statusCodesToIgnore?.Contains(httpStatus) == true;
    }

    public async Task<ApiResult<bool>> ExecuteAndHandleExceptionsAsync(Func<Task> apiTask,
        bool ignoreExceptions = false, IEnumerable<int>? statusCodesToIgnore = null)
    {
        var apiResult = await ExecuteAndHandleExceptionsAsync(async () =>
        {
            await apiTask();
            return true;
        }, ignoreExceptions, statusCodesToIgnore);
        return apiResult;
    }

    private async Task NotifyAsync(string message, bool isError)
    {
        await alertNotifier.ShowAlertAsync(message, isError ?"Error happened" : "Message", isError);
    }
}