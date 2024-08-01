namespace Codebase.Communication;

public enum CommPySign : byte 
{
    Start = 0x05,
}

public enum CommPySender : byte
{
    Logging,
    IoControl,
    Controller,
}

public enum CommPyReceiver : byte 
{
    Logging,
    IoControl,
    Controller,
    Broadcast
}

public enum CommPyPriority : byte
{
    High,
    Middle,
    Low,
    Inform
}

public enum CommPyEncoding : byte
{
    Ascii,
    Utf8
}