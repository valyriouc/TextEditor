
using Codebase.Communication;
using System.Reflection;

enum LogLevel
{
    Inform,
    Warn,
    Error,
}

internal static class LogLevelExtensions
{
    public static char ShortSign(this LogLevel self) => self switch
    {
        LogLevel.Inform => 'I',
        LogLevel.Warn => 'W',
        LogLevel.Error => 'E',
        _ => throw new NotImplementedException()
    };

    public static ConsoleColor GetConsoleColor(this LogLevel self) => self switch
    {
        LogLevel.Inform => ConsoleColor.Green,
        LogLevel.Warn => ConsoleColor.Yellow,
        LogLevel.Error => ConsoleColor.Red,
        _ => throw new NotImplementedException()
    };
}

struct LoggingEntry
{
    public DateTime TimeStamp { get; }

    // TODO: We need the name of the process where the log request came from!

    public LogLevel Level { get; }
    
    public string Content { get; }

    public LoggingEntry(LogLevel level, string content)
    {
        TimeStamp = DateTime.UtcNow;
        Level = level;
        Content = content;
    }

    public override string ToString()
    {
        return $"{TimeStamp.ToString("yyyy-MM-dd_hh:mm:ss.fff")},{Level.ShortSign()},{Content}";
    }
}

/// <summary>
/// Executes logging tasks
/// Maybe csv is better or a custom format 
/// </summary>
internal class Program
{
    private static string ExecutionPath { get; } = 
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? 
            throw new Exception("Could not get the directory of the executing assembly!");

    private static string LogPath { get; } =
        Path.Combine(ExecutionPath, "logs");

    public static async Task Main()
    {
        using CommManager manager = new CommManager(
            Console.OpenStandardInput(), 
            Console.OpenStandardOutput());

        DateTime startTime = DateTime.UtcNow;
        string logFile = Path.Combine(LogPath, $"{startTime.ToString("yyyy-MM-dd_hh:mm:ss")}.csv");
        FileStream stream;
        if (!File.Exists(logFile))
        {
            stream = File.OpenWrite(logFile);
            // TODO: Signal success 
        }
        else
        {
            // TODO: Signal failure
            return;
        }


    }
}

internal interface ICommHandable
{
    public void HandleMessage(CommPyMessage message);
}

internal class CommManager : IDisposable
{
    private static CancellationTokenSource Cts { get; } 
        = new CancellationTokenSource();

    public static CancellationToken MainToken => Cts.Token;

    private Stream Stdin { get; }

    private Stream Stdout { get; }

    private ICommHandable Handler { get; }

    public CommManager(Stream stdin, Stream stdout, ICommHandable handler)
    {
        Stdin = stdin;
        Stdout = stdout;
        Handler = handler;
    }

    public void Start()
    {
        Thread t = new Thread(Read);
        t.IsBackground = true;
        t.Start();
    }

    public void Read()
    {
        int count = 0;
        Span<byte> buffer = new byte[1024];
        while (!MainToken.IsCancellationRequested && (count = Stdin.Read(buffer)) > 0)
        {
            if (CommPyMessage.TryReadFrom(buffer, out CommPyMessage msg, out ReadOnlySpan<byte> remaining))
            {
                
            }
        }
    }

    public void SignalReady()
    {

    }

    public void SignalFailure()
    {

    }

    public void Dispose()
    {
        Stdin.Dispose();
        Stdout.Dispose();
    }

}