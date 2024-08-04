namespace Codebase.Communication;

public enum ParsingState
{
    NoMessageStart,
    MissingData,
    InvalidSender,
    InvalidReceiver,
    InvalidPriority,
    Success
}

public interface IMessageContent<T>
{
    public Memory<byte> AsByteSequence();
    
    public abstract static T FromByteSequence(Memory<byte> bytes);
}

public static class MemoryExtensions
{
    public static bool TryAs<T>(this Memory<byte> self, out T item)
        where T : IMessageContent<T>
    {
        item = default;
        try
        {
            item = T.FromByteSequence(self);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}

public struct CommPyMessage
{
    private static byte StartByte => 0x1f; // Unit Separator
   
    public CommPySender Sender { get; }

    public CommPyReceiver Receiver { get; }

    public CommPyPriority Priority { get; }

    public byte Command { get; }

    public Memory<byte> Content { get; }

    public CommPyMessage(
        CommPySender sender, 
        CommPyReceiver receiver,
        CommPyPriority priority,
        byte command) 
        : this(sender, receiver, priority, command, Array.Empty<byte>())
    {
        
    }

    private CommPyMessage(
        CommPySender sender,
        CommPyReceiver receiver,
        CommPyPriority priority,
        byte command,
        Memory<byte> content)
    {
        Sender = sender;
        Receiver = receiver;
        Priority = priority;
        Command = command;
        Content = content;
    }
    
    public static ParsingState TryReadFrom(
        ReadOnlySpan<byte> buffer, 
        out CommPyMessage msg, 
        out ReadOnlySpan<byte> offset)
    {
        // TODO: Needs to be implemented
        msg = default;
        offset = buffer;

        if (buffer.IsEmpty || buffer[0] != CommPyMessage.StartByte) 
        {
            return ParsingState.NoMessageStart;
        }

        buffer = buffer[1..];
        
        return ParsingState.Success;
    }

    public int Write(Span<byte> buffer)
    {
        Span<byte> length = BitConverter.GetBytes(Content.Length);
        
        if (buffer.Length < (5 + length.Length + Content.Length))
        {
            throw new Exception("Buffer is to small!");
        }

        buffer[0] = StartByte;
        buffer[1] = (byte)Sender;
        buffer[2] = (byte)Receiver;
        buffer[3] = (byte)Priority;
        buffer[4] = Command;

        int wrote = 5;

        Content.Span.CopyTo(buffer[5..9]);

        wrote += length.Length;

        Content.Span.CopyTo(buffer[9..]);
        wrote += Content.Length;

        return wrote;
    }
 }

file static class ByteExtensions
{
    public static bool TryAsSender(this byte self, out CommPySender sender)
    {
        sender = default;
        switch (self)
        {
            case byte n when n >= 0x00 && n <= 0x02:
                sender = (CommPySender)self;
                return true;
            default:
                return false;
        }
    }

    public static bool TryAsReceiver(this byte self, out CommPyReceiver receiver)
    {
        receiver = default;
        switch (self)
        {
            case byte n when n >= 0x00 && n <= 0x03:
                receiver = (CommPyReceiver)self;
                return true;
            default:
                return false;
        }
    }
    
    public static bool TryAsPriority(this byte self, out CommPyPriority priority)
    {
        priority = default;
        switch (self)
        {
            case byte n when n >= 0x00 && n <= 0x03:
                priority = (CommPyPriority)self;
                return true;
            default:
                return false;
        }
    }
}
