

using Codebase.Communication;
using Controller;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

/// <summary>
/// Spawns all necessary processes 
/// Manages stdio messages an redirect them 
/// </summary>
internal enum ProcessType
{
    IoControl,
    Logging
}

/// <summary>
/// We need a thread save message system to handle incoming stuff
/// </summary>
internal class Program
{
    private static ConcurrentDictionary<ProcessType, Process> Processes { get; } 
        = new ConcurrentDictionary<ProcessType, Process>();
    
    private static void Init()
    {

    }

    public static async Task Main(string[] args)
    {
        Init();

        ConcurrentQueue<CommPyMessage> messages = new ConcurrentQueue<CommPyMessage>();

        foreach (Process process in Processes.Values)
        {
            Thread t = new Thread(() =>
            {
                Span<byte> buffer = new byte[1024];
                Stream stdout = process.StandardOutput.BaseStream;
                int offset = 0;
                while (true)
                {
                    int count = stdout.Read(buffer);
                    if (CommPyMessage.TryReadFrom(buffer[..count], out CommPyMessage msg, out offset))
                    {
                        Console.WriteLine("Successfully got the message!");
                        messages.Enqueue(msg);
                    }
                }
            });

            t.IsBackground = true;
            t.Start();
        }

        // Ein Thread fürs priorisierte Routing und Schreiben 

        // For every process we need reading and writing 

        // Create processes 

        // handle input and output 
    }
}
