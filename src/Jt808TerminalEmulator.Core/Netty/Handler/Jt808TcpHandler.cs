using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using GpsPlatform.Infrastructure.Extentions;
using GpsPlatform.Jt808Protocol;
using GpsPlatform.Jt808Protocol.Instruction;
using GpsPlatform.Jt808Protocol.PackageInfo;
using Jt808TerminalEmulator.Core.Abstract;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jt808TerminalEmulator.Core.Netty.Handler
{
    internal class Jt808TcpHandler : SimpleChannelInboundHandler<byte[]>
    {
        private readonly ILogger logger;
        private readonly IPackageConverter packageConverter;
        private readonly ISessionManager sessionManager;

        public Jt808TcpHandler(ILogger<Jt808TcpHandler> logger, IPackageConverter packageConverter, ISessionManager sessionManager)
        {
            this.logger = logger;
            this.packageConverter = packageConverter;
            this.sessionManager = sessionManager;
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, byte[] msg)
        {
            try
            {
                var package = packageConverter.Deserialize<Jt808PackageInfo>(msg);
                if (logger.IsEnabled(LogLevel.Trace))
                    logger.LogTrace($"解析成功--->卡号：{package.Header.PhoneNumber} ，消息：{msg.ToHexString()}");

            }
            catch (Exception e)
            {
                if (logger.IsEnabled(LogLevel.Error))
                    logger.LogError(e, $"数据解析出现异常，元数据：{msg.ToHexString()}");
            }
        }

        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            switch (evt)
            {
                case IdleStateEvent readerIdle when readerIdle.State == IdleState.ReaderIdle:
                    context.CloseAsync();
                    break;
                case IdleStateEvent writerIdle when writerIdle.State == IdleState.WriterIdle:
                    var session = sessionManager.GetTcpClientSessions().FirstOrDefault(x => x.Id == context.Channel.Id.AsLongText());
                    if (session != default)
                        context.WriteAndFlushAsync(new Jt808PackageInfo
                        {
                            Header = new Header
                            {
                                PhoneNumber = session.PhoneNumber
                            },
                            Body = new Jt808_0x0002_Heartbeat()
                        });
                    break;
            }

            base.UserEventTriggered(context, evt);
        }
    }
}
