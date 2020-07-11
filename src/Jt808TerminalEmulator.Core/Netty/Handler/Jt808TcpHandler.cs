using DotNetty.Transport.Channels;
using GpsPlatform.Infrastructure.Extentions;
using GpsPlatform.Jt808Protocol;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jt808TerminalEmulator.Core.Netty.Handler
{
    internal class Jt808TcpHandler : SimpleChannelInboundHandler<byte[]>
    {
        private readonly ILogger logger;
        private readonly PackageConverter packageConverter;

        public Jt808TcpHandler(ILogger<Jt808TcpHandler> logger, PackageConverter packageConverter)
        {
            this.logger = logger;
            this.packageConverter = packageConverter;
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, byte[] msg)
        {
            try
            {
                var package = packageConverter.Deserialize(msg);
                if (logger.IsEnabled(LogLevel.Trace))
                    logger.LogTrace($"解析成功--->卡号：{package.Header.PhoneNumber} ，消息：{msg.ToHexString()}");

            }
            catch (Exception e)
            {
                if (logger.IsEnabled(LogLevel.Error))
                    logger.LogError(e, $"数据解析出现异常，元数据：{msg.ToHexString()}");
            }
        }
    }
}
