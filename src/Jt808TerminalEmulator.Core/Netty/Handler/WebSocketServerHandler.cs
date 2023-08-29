using System.Text;
using DotNetty.Buffers;
using DotNetty.Codecs.Http;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Common.Utilities;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using static DotNetty.Codecs.Http.HttpResponseStatus;
using static DotNetty.Codecs.Http.HttpVersion;

namespace Jt808TerminalEmulator.Core.Netty.Handler;

public class WebSocketServerHandler : SimpleChannelInboundHandler<object>
{
    readonly ILogger logger;
    WebSocketServerHandshaker handshaker;
    private readonly WebSocketSessionManager sessionManager;

    public WebSocketServerHandler(ILogger<WebSocketServerHandler> logger, WebSocketSessionManager sessionManager)
    {
        this.logger = logger;
        this.sessionManager = sessionManager;
    }

    protected override void ChannelRead0(IChannelHandlerContext ctx, object msg)
    {
        switch (msg)
        {
            case IFullHttpRequest request:
                HandleHttpRequest(ctx, request);
                break;
            case WebSocketFrame frame:
                HandleWebSocketFrame(ctx, frame);
                break;
        }
    }

    public override void UserEventTriggered(IChannelHandlerContext context, object evt)
    {
        string channelId = context.Channel.Id.AsLongText();
        switch (evt)
        {
            case IdleStateEvent state when state.State == IdleState.ReaderIdle:
                // 读取超时主动关闭连接。
                if (sessionManager.TryRemove(channelId))//仅打印websocket握手成功的连接
                    logger.LogInformation($"{channelId} The websocket server read time out.");
                break;
        }
        base.UserEventTriggered(context, evt);
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
    {
        base.ExceptionCaught(context, exception);
        sessionManager.TryRemove(context.Channel.Id.AsLongText());
    }

    public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

    /// <summary>
    /// http请求，用于握手
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="req"></param>
    void HandleHttpRequest(IChannelHandlerContext ctx, IFullHttpRequest req)
    {
        // 无效请求.
        if (!req.Result.IsSuccess)
        {
            SendHttpResponse(ctx, req, new DefaultFullHttpResponse(Http11, BadRequest));
            return;
        }

        //图标资源
        if ("/favicon.ico".Equals(req.Uri))
        {
            //返回200
            var res = new DefaultFullHttpResponse(Http11, OK);
            SendHttpResponse(ctx, req, res);
            return;
        }

        // 握手
        handshaker = new WebSocketServerHandshakerFactory(GetWebSocketLocation(req), null, true, 5 * 1024 * 1024).NewHandshaker(req);
        if (handshaker == null)//握手失败
        {
            //发送不支持的版本响应
            WebSocketServerHandshakerFactory.SendUnsupportedVersionResponse(ctx.Channel);
        }
        else
        {
            handshaker.HandshakeAsync(ctx.Channel, req);
            sessionManager.TryAdd(new WebSocketSession(ctx.Channel));
        }
    }

    /// <summary>
    /// websocket帧处理
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="frame"></param>
    void HandleWebSocketFrame(IChannelHandlerContext ctx, WebSocketFrame frame)
    {
        switch (frame)
        {
            case CloseWebSocketFrame close:
                handshaker.CloseAsync(ctx.Channel, close);
                sessionManager.TryRemove(ctx.Channel.Id.AsLongText());
                break;
            case PingWebSocketFrame ping:
                ctx.WriteAndFlushAsync(new PongWebSocketFrame((IByteBuffer)ping.Content.Retain()));
                break;
            case TextWebSocketFrame:
                sessionManager.UpdateLastActiveTime(ctx.Channel.Id.AsLongText());
                break;
            case BinaryWebSocketFrame binary:
                sessionManager.UpdateLastActiveTime(ctx.Channel.Id.AsLongText());
                ctx.WriteAndFlushAsync(binary.Retain());
                break;
        }
    }

    private static void SendHttpResponse(IChannelHandlerContext ctx, IFullHttpRequest req, IFullHttpResponse res)
    {
        // 如果响应getStatus代码不正确，则生成错误页面
        if (res.Status.Code != 200)
        {
            IByteBuffer buf = Unpooled.CopiedBuffer(Encoding.UTF8.GetBytes(res.Status.ToString()));
            res.Content.WriteBytes(buf);
            buf.Release();
            HttpUtil.SetContentLength(res, res.Content.ReadableBytes);
        }

        // 发送响应并在必要时关闭连接
        Task task = ctx.Channel.WriteAndFlushAsync(res);
        if (!HttpUtil.IsKeepAlive(req) || res.Status.Code != 200)
        {
            task.ContinueWith((_, c) => ((IChannelHandlerContext)c).CloseAsync(), ctx, TaskContinuationOptions.ExecuteSynchronously);
        }
    }

    private static string GetWebSocketLocation(IFullHttpRequest req)
    {
        req.Headers.TryGet(HttpHeaderNames.Host, out ICharSequence value);
        return "ws://" + value.ToString();
    }
}