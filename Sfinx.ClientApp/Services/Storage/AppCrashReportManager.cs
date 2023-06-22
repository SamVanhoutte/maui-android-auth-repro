using Microsoft.AspNetCore.Components;
using Sfinx.App.Shared.Interfaces;
using Sfinx.App.Shared.Models;
using Sfinx.App.Shared.Models.ErrorHandling;
using Sfinx.Backend.WebAPI.Client;
using Exception = System.Exception;

namespace Sfinx.ClientApp.Services.Storage;

public class AppCrashReportManager : ICrashReportHandler
{
    private readonly IStorageCache storageCacheHandler;
    private readonly IUserProfileManager profileManager;
    private readonly NavigationManager navigationManager;
    private const string CrashReportCollectionName = "crashreports";
    
    public AppCrashReportManager(IStorageCache storageCacheHandler, IUserProfileManager profileManager,
        NavigationManager navigationManager)
    {
        this.storageCacheHandler = storageCacheHandler;
        this.profileManager = profileManager;
        this.navigationManager = navigationManager;
    }

    public async Task<ExceptionHandlingResult> HandleExceptionAsync(Exception ex)
    {
        var userId = await profileManager.GetUserIdAsync();
        var timestamp = DateTime.UtcNow;
        var report = new CrashReport
        {
            ReportId = Guid.NewGuid(),
            UserId = userId,
            PageUri = null,
            Timestamp = timestamp,
            ErrorMessage = ex?.Message,
            Stacktrace = ex?.StackTrace,
            Context = new Dictionary<string, string>()
        };
        if (ex is ApiException apiEx)
        {
            report.IsApiException = true;
            report.ApiStatusCode = apiEx.StatusCode;
            report.ApiResponse = apiEx.Response;
        }
        await storageCacheHandler.AddFolderItemAsync(CrashReportCollectionName, $"{Guid.NewGuid():N}", report);
        return new ExceptionHandlingResult{Message = ex.Message, ReportId = report.ReportId};
    }

    public async Task<IEnumerable<CrashReport>> GetCrashReportsAsync(DateTime? fromDate = null)
    {
        var reports = await storageCacheHandler.GetFolderItemsAsync<CrashReport>(CrashReportCollectionName);
        return reports.Where(r => r.Timestamp >= (fromDate ?? DateTime.MinValue));
    }

    public async Task<CrashReport> GetCrashReportAsync(Guid reportId)
    {
        var reports = await GetCrashReportsAsync();
        return reports.FirstOrDefault(r => r.ReportId.Equals(reportId));
    }

    public async Task ClearAllAsync()
    {
        await storageCacheHandler.ClearFolderItemsAsync(CrashReportCollectionName);
    }
}