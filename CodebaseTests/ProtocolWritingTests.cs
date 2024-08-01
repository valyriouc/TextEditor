using Codebase.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public void ThrowWhenBufferToSmall(int count)
    {
        CommPyMessage msg = new CommPyMessage();
        Span<byte> buffer = new byte[count];
    }

    [Fact]
    public void SuccessfulWriteMessageToBuffer()
    {

    }
}
