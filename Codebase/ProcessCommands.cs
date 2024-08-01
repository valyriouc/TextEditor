namespace Codebase;

public enum IoControlCommands : byte
{
    List,
    Load,
    Ready,
}

public enum LoggerCommands : byte
{
    Log,
    Ready,
}

public enum ControllingCommands : byte
{
    Status,
    Restart,
    Shutdown,
}
