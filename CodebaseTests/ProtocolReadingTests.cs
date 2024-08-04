using Xunit;
using System;
using Codebase.Communication;

public class CommPyMessageTests
{
    [Fact]
    public void TryReadFrom_EmptyBuffer_ReturnsNoMessageStart()
    {
        ReadOnlySpan<byte> buffer = Array.Empty<byte>();
        CommPyMessage result;
        ReadOnlySpan<byte> offset;

        var state = CommPyMessage.TryReadFrom(buffer, out result, out offset);

        Assert.Equal(ParsingState.NoMessageStart, state);
    }

    [Fact]
    public void TryReadFrom_InvalidStartByte_ReturnsNoMessageStart()
    {
        ReadOnlySpan<byte> buffer = new byte[] { 0x00 }; // not 0x1f
        CommPyMessage result;
        ReadOnlySpan<byte> offset;

        var state = CommPyMessage.TryReadFrom(buffer, out result, out offset);

        Assert.Equal(ParsingState.NoMessageStart, state);
    }

    [Fact]
    public void TryReadFrom_InvalidSender_ReturnsInvalidSender()
    {
        ReadOnlySpan<byte> buffer = new byte[] { 0x1f, 0x04 }; // invalid sender
        CommPyMessage result;
        ReadOnlySpan<byte> offset;

        ParsingState state = CommPyMessage.TryReadFrom(buffer, out result, out offset);

        Assert.Equal(ParsingState.InvalidSender, state);
    }
    
    [Fact]
    public void TryReadFrom_ValidBuffer_ReturnsSuccess()
    {
        ReadOnlySpan<byte> buffer = new byte[]
        {
            0x1f, // StartByte
            0x00, // CommPySender.Logging
            0x01, // CommPyReceiver.IoControl
            0x01, // CommPyPriority.Middle
            0x20, // random command
            // length - four bytes representing fixed value of 4
            0x04, 0x00, 0x00, 0x00,
            0x01, 0x02, 0x03, 0x04 // data of length 4
        };

        ParsingState state = CommPyMessage.TryReadFrom(buffer, out CommPyMessage result, out ReadOnlySpan<byte> offset);

        Assert.Equal(ParsingState.Success, state);
        Assert.Equal(CommPySender.Logging, result.Sender);
        Assert.Equal(CommPyReceiver.IoControl, result.Receiver);
        Assert.Equal(CommPyPriority.Middle, result.Priority);
        Assert.Equal(0x20, result.Command);
        Assert.Equal(4, result.Content.Length);
        Assert.Equal(new byte[] { 0x01, 0x02, 0x03, 0x04 }, result.Content.ToArray());
    }
}