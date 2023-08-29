using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using GpsPlatform.Infrastructure.Extentions;
using Microsoft.Extensions.Logging;

namespace Jt808TerminalEmulator.Core.Netty.Codec;

internal class Jt808Decoder : ByteToMessageDecoder
{
    private readonly ILogger logger;
    public Jt808Decoder(ILogger<Jt808Decoder> logger)
    {
        this.logger = logger;
    }
    protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
    {
        if (input.ReadableBytes >= 12)
        {
            byte[] buffer = new byte[input.ReadableBytes + 2];
            input.ReadBytes(buffer, 1, input.ReadableBytes);
            buffer[0] = 0x7e;
            buffer[input.ReaderIndex - input.ReadableBytes + 1] = 0x7e;
            output.Add(buffer);
        }
        else
        {
            if (logger.IsEnabled(LogLevel.Error))
                logger.LogError($"收到无效数据包：{input.ReadBytes(input.ReadableBytes).Array.ToHexString()}");
        }
    }
}