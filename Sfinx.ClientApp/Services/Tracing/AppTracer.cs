
using System.Text;

namespace Sfinx.ClientApp.Services.Tracing;

public class AppTracer
{
    private static Stack<AppTrace> traces = new Stack<AppTrace>();
    public const int MaxLineCount = 50;
    
    public void Trace(string message)
    {
        traces.Push(new AppTrace
        {
            Timestamp = DateTime.Now,
            Message = message
        });
        if (traces.Count >= MaxLineCount)
        {
            traces.TryPop(out _);
        }
    }
    
    public string GetTrace()
    {
        var sb = new StringBuilder();
        foreach (var trace in traces)
        {
            sb.AppendLine($"{trace.Timestamp:HH:mm:ss.fff} - {trace.Message}");
        }
        return sb.ToString();
    }
}

public class AppTrace
{
    public DateTime Timestamp { get; set; }
    public string Message { get; set; }
    public string ExceptionMessage { get; set; }
}