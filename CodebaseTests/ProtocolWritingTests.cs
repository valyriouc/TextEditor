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
            CommPyPriority.Inform);

            Span<byte> buffer = new byte[count];
            msg.Write(buffer);
        }

        Assert.Throws<Exception>(() => Wrapper());
    }

    [Fact]
    public void SuccessfulWriteMessageToBuffer()
    {

    }
}
