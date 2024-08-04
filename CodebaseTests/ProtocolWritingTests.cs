using Codebase.Communication;

namespace CodebaseTests;

public class ProtocolWritingTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    public void ThrowWhenBufferToSmallForMessageWithoutContent(int count)
    {
        void Wrapper()
        {
            CommPyMessage msg = new CommPyMessage(
            CommPySender.Controller,
            CommPyReceiver.Broadcast,
            CommPyPriority.Inform,
            0x20);

            Span<byte> buffer = new byte[count];
            msg.Write(buffer);
        }

        Assert.Throws<Exception>(() => Wrapper());
    }

    [Fact]
    public void SuccessfulWriteMessageToBuffer()
    {
        CommPyMessage msg = new CommPyMessage(
            CommPySender.Controller,
            CommPyReceiver.Broadcast,
            CommPyPriority.Inform,
            0x20);

        Span<byte> buffer = new byte[1024];
        int count = msg.Write(buffer);
        
        Assert.Equal(9, count);

        byte[] expected =
        [
            0x1f,
            0x02,
            0x03,
            0x03,
            0x20,
            0x00,
            0x00,
            0x00,
            0x00
        ];
        
        Assert.Equal(expected, buffer[..count].ToArray());
    }
}
