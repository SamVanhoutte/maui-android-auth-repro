using Microsoft.Extensions.Logging;
using Sfinx.App.Shared.Interfaces;

namespace Sfinx.App.Shared.Services.ErrorHandling;

public class UnhandledExceptionProcessor : IUnhandledExceptionProcessor
{
    public event EventHandler<Exception>? UnhandledExceptionThrown;
    
    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
        Exception? exception, Func<TState, Exception, string> formatter)
    {
        if (exception != null)
        {
            UnhandledExceptionThrown?.Invoke(this, exception);
        }
    }
}