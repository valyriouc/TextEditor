namespace Codebase.Communication;

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

    public byte ProcessCommand { get; }

    public Memory<byte> Content { get; }

    public CommPyMessage(
        CommPySender sender, 
        CommPyReceiver receiver,
        CommPyPriority priority) 
        : this(sender, receiver, priority, Array.Empty<byte>())
    {

    }

    private CommPyMessage(
        CommPySender sender,
        CommPyReceiver receiver,
        CommPyPriority priority,
        Memory<byte> content)
    {
        Sender = sender;
        Receiver = receiver;
        Priority = priority;
        Content = content;
    }



    public static bool TryReadFrom(
        ReadOnlySpan<byte> buffer, 
        out CommPyMessage msg, 
        out ReadOnlySpan<byte> offset)
    {
        // TODO: Needs to be implemented
        msg = default;
        offset = buffer;
        return true;
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
        buffer[4] = (byte)ProcessCommand;

        int wrote = 5;

        Content.Span.CopyTo(buffer[5..9]);

        wrote += length.Length;

        Content.Span.CopyTo(buffer[9..]);
        wrote += Content.Length;

        return wrote;
    }
 }
