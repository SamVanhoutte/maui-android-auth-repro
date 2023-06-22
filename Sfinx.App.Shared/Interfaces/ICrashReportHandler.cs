using Sfinx.App.Shared.Models;
using Sfinx.App.Shared.Models.ErrorHandling;

namespace Sfinx.App.Shared.Interfaces;

public interface ICrashReportHandler
{
    Task<ExceptionHandlingResult> HandleExceptionAsync(Exception ex);
    Task<IEnumerable<CrashReport>> GetCrashReportsAsync(DateTime? fromDate = null);
    Task<CrashReport> GetCrashReportAsync(Guid reportId);
    Task ClearAllAsync();
}