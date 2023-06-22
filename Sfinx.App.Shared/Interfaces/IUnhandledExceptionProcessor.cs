namespace Sfinx.App.Shared.Interfaces;

public interface IUnhandledExceptionProcessor
{
    event EventHandler<Exception> UnhandledExceptionThrown;
}