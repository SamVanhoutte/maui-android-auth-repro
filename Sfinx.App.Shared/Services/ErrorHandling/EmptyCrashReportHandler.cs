using Sfinx.App.Shared.Interfaces;
using Sfinx.App.Shared.Models.ErrorHandling;

namespace Sfinx.App.Shared.Services.ErrorHandling;

public class EmptyCrashReportHandler : ICrashReportHandler
{
    public Task<ExceptionHandlingResult> HandleExceptionAsync(Exception ex)
    {
        return Task.FromResult<ExceptionHandlingResult>(new ExceptionHandlingResult());
    }

    public Task<IEnumerable<CrashReport>> GetCrashReportsAsync(DateTime? fromDate = null)
    {
        return Task.FromResult<IEnumerable<CrashReport>>(new List<CrashReport>());
    }

    public Task<CrashReport> GetCrashReportAsync(Guid reportId)
    {
        return Task.FromResult(new CrashReport());
    }

    public Task ClearAllAsync()
    {
        return Task.CompletedTask;
    }
}