using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using GpsPlatform.Infrastructure.Extentions;
using GpsPlatform.Jt808Protocol.Instruction;
using GpsPlatform.Jt808Protocol.PackageInfo;
using Jt808TerminalEmulator.Core.Abstract;
using Microsoft.Extensions.Logging;

namespace Jt808TerminalEmulator.Core.Netty.Handler;

internal class Jt808TcpHandler : SimpleChannelInboundHandler<byte[]>
{
    private readonly ILogger logger;
    private readonly IPackageConverter packageConverter;
    private readonly ISessionManager sessionManager;
    private string phoneNumber;

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
            if (logger.IsEnabled(LogLevel.Trace))
                logger.LogTrace("{phoneNumber} 收到报文: {hex}", phoneNumber, msg.ToHexString());
            if (phoneNumber == null)
            {
                var session = sessionManager.GetTcpClientSessionById(ctx.Channel.Id.AsLongText());
                phoneNumber = session?.PhoneNumber;
            }
            var package = packageConverter.Deserialize<Jt808PackageInfo>(msg);
            switch (package.Body)
            {
                case Jt808_0x8001_UniversalResponse jt808_0x0001:
                    {
                        switch (jt808_0x0001.MessageId)
                        {
                            case 0x0102:
                                {
                                    logger.LogDebug("{phoneNumber}鉴权结果：{result}", phoneNumber, jt808_0x0001.Result.ToDescription());
                                    if (jt808_0x0001.Result == Jt808_0x8001_UniversalResponse.ResultType.Success && sessionManager.GetTcpClientSessionById(ctx.Channel.Id.AsLongText()) is TcpClientSession session)
                                    {
                                        session.Logged = true;
                                    }
                                }
                                break;
                            case 0x0003:
                                {
                                    logger.LogDebug("{phoneNumber}注销结果：{result}", phoneNumber, jt808_0x0001.Result.ToDescription());
                                    ctx.WriteAndFlushAsync(new Jt808PackageInfo
                                    {
                                        Header = new Header { PhoneNumber = phoneNumber },
                                        Body = new Jt808_0x0100_Register
                                        {
                                            ManufacturerId = "",
                                            TerminalModel = "",
                                            TerminalId = phoneNumber,
                                            VehicleIdentification = ""
                                        }
                                    });
                                }
                                break;
                        }
                    }
                    break;
                case Jt808_0x8100_RegisterRespone jt808_0x8100:
                    {
                        logger.LogDebug("{phoneNumber}注册结果：{result}", phoneNumber, jt808_0x8100.Result.ToDescription());
                        var data = new Jt808PackageInfo
                        {
                            Header = new Header { PhoneNumber = phoneNumber },
                            Body = jt808_0x8100.Result switch
                            {
                                Jt808_0x8100_RegisterRespone.RegisterResult.Success => new Jt808_0x0102_Authentication
                                {
                                    Code = jt808_0x8100.Code
                                },
                                _ => new Jt808_0x0003_Logout()
                            }
                        };
                        ctx.WriteAndFlushAsync(data);
                    }
                    break;
            }
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
                context.WriteAndFlushAsync(new Jt808PackageInfo
                {
                    Header = new Header
                    {
                        PhoneNumber = phoneNumber
                    },
                    Body = new Jt808_0x0002_Heartbeat()
                });
                break;
        }

        base.UserEventTriggered(context, evt);
    }
}