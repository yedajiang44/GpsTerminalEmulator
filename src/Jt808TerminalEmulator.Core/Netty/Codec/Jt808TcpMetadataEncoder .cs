using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using GpsPlatform.Jt808Protocol;
using GpsPlatform.Jt808Protocol.PackageInfo;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jt808TerminalEmulator.Core.Netty.Codec
{
    internal class Jt808Encoder : MessageToByteEncoder<Jt808PackageInfo>
    {
        readonly ILogger logger;
        readonly PackageConverter packageConverter;

        public Jt808Encoder(ILogger<Jt808Encoder> logger, PackageConverter packageConverter)
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
                    output.WriteBytes(Unpooled.WrappedBuffer(packageConverter.Serialize(message)));
                }
                catch (Exception e)
                {
                    if (logger.IsEnabled(LogLevel.Error))
                        logger.LogError(e, $"消息 {message.Header.MessageId:X2} 编码发生异常");
                }
            }
        }
    }
}
