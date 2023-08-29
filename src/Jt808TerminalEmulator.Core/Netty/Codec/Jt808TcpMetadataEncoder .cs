using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using GpsPlatform.Jt808Protocol.PackageInfo;
using Jt808TerminalEmulator.Core.Abstract;
using Microsoft.Extensions.Logging;

namespace Jt808TerminalEmulator.Core.Netty.Codec;

internal class Jt808Encoder : MessageToByteEncoder<Jt808PackageInfo>
{
    readonly ILogger logger;
    readonly IPackageConverter packageConverter;

    public Jt808Encoder(ILogger<Jt808Encoder> logger, IPackageConverter packageConverter)
    {
        this.logger = logger;
        this.packageConverter = packageConverter;
    }

    protected override void Encode(IChannelHandlerContext context, Jt808PackageInfo message, IByteBuffer output)
    {
        if (message != null)
        {
            try
            {
                output.MarkWriterIndex();
                output.WriteBytes(Unpooled.WrappedBuffer(packageConverter.Serialize(message)));
            }
            catch (Exception e)
            {
                output.ResetWriterIndex();
                if (logger.IsEnabled(LogLevel.Error))
                    logger.LogError(e, $"消息 {message.Header.MessageId:X2} 编码发生异常");
            }
        }
    }
}