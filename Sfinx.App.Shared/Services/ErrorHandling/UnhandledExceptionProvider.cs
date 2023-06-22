using Microsoft.Extensions.Logging;
using Sfinx.App.Shared.Interfaces;

namespace Sfinx.App.Shared.Services.ErrorHandling;

public class UnhandledExceptionProvider : ILoggerProvider
{
    readonly IUnhandledExceptionProcessor unhandledExceptionSender;
    private readonly ICrashReportHandler crashReportHandler;


    public UnhandledExceptionProvider(IUnhandledExceptionProcessor unhandledExceptionSender, ICrashReportHandler crashReportHandler)
    {
        this.unhandledExceptionSender = unhandledExceptionSender;
        this.crashReportHandler = crashReportHandler;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new UnhandledExceptionLogger(categoryName, unhandledExceptionSender, crashReportHandler);
    }

    public void Dispose()
    {            
    }

    public class UnhandledExceptionLogger : ILogger
    {
        private readonly string categoryName;
        private readonly IUnhandledExceptionProcessor unhandeledExceptionSender;
        private readonly ICrashReportHandler crashReportHandler;

        public UnhandledExceptionLogger(string categoryName, IUnhandledExceptionProcessor unhandledExceptionSender, ICrashReportHandler crashReportHandler)
        {
            unhandeledExceptionSender = unhandledExceptionSender;
            this.crashReportHandler = crashReportHandler;
            this.categoryName = categoryName;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            // Unhandled exceptions will call this method
            // Blazor already logs unhandled exceptions to the browser console
            // but, one could pass the exception to the server to log, this is easily done with serilog
            if (crashReportHandler != null)
            {
                crashReportHandler.HandleExceptionAsync(exception).Wait();
            }
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new NoopDisposable();
        }

        private class NoopDisposable : IDisposable
        {
            public void Dispose()
            {  
            }
        }
    }
}