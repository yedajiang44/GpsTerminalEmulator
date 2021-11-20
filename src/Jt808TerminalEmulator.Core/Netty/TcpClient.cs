using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using GpsPlatform.Jt808Protocol.PackageInfo;
using Jt808TerminalEmulator.Core.Abstract;
using Jt808TerminalEmulator.Core.Netty.Codec;
using Jt808TerminalEmulator.Core.Netty.Handler;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Jt808TerminalEmulator.Core.Netty
{
    internal class TcpClient : ITcpClient
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        readonly MultithreadEventLoopGroup eventLoopGroup;
        readonly Bootstrap bootstrap;
        readonly ISessionManager sessionManager;
        readonly IServiceProvider serviceProvider;


        public TcpClient(IServiceProvider serviceProvider, ISessionManager sessionManager)
        {
            this.sessionManager = sessionManager;
            this.serviceProvider = serviceProvider;
            eventLoopGroup = new MultithreadEventLoopGroup();
            bootstrap = new Bootstrap().Group(eventLoopGroup)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Option(ChannelOption.SoReuseaddr, true)
                .Option(ChannelOption.SoKeepalive, true)
                .Option(ChannelOption.ConnectTimeout, TimeSpan.FromSeconds(300))
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    var scope = serviceProvider.CreateScope().ServiceProvider;
                    IChannelPipeline pipeline = channel.Pipeline;
                    pipeline.AddLast(new IdleStateHandler(3000, 5, 0));
                    pipeline.AddLast(new DelimiterBasedFrameDecoder(1024, Unpooled.CopiedBuffer(new byte[] { 0x7e }), Unpooled.CopiedBuffer(new byte[] { 0x7e })));
                    pipeline.AddLast(scope.GetRequiredService<Jt808Encoder>());
                    pipeline.AddLast(scope.GetRequiredService<Jt808Decoder>());
                    pipeline.AddLast(scope.GetRequiredService<Jt808TcpHandler>());
                }));
        }


        public Task<ITcpClientSession> ConnectAsync(string ip, int port, string phoneNumber = null)
        {
            return bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(ip), port)).ContinueWith(task =>
            {
                ITcpClientSession session = new TcpClientSession(serviceProvider) { Channel = task.Result, PhoneNumber = phoneNumber };
                sessionManager.Add(session);
                return session;
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public Task Send(string phoneNumber, Jt808PackageInfo data) => sessionManager.GetTcpClientSession(phoneNumber).Send(data);
        public Task Send(string phoneNumber, byte[] data) => sessionManager.GetTcpClientSession(phoneNumber).Send(data);

        public Task<IEnumerable<ITcpClientSession>> Sesions() => Task.FromResult(sessionManager.GetTcpClientSessions());

        public Task<ITcpClientSession> GetSessionById(string sessionId) => Task.FromResult(sessionManager.GetTcpClientSessionById(sessionId));
        public Task<ITcpClientSession> GetSession(string phoneNumber) => Task.FromResult(sessionManager.GetTcpClientSession(phoneNumber));
    }

    public interface ITcpClient
    {
        public string Id { get; set; }
        Task<ITcpClientSession> ConnectAsync(string ip, int port, string phoneNumber = null);
        Task<IEnumerable<ITcpClientSession>> Sesions();
        Task<ITcpClientSession> GetSession(string phoneNumber);
        Task<ITcpClientSession> GetSessionById(string sessionId);
        Task Send(string phoneNumber, Jt808PackageInfo data);
        Task Send(string phoneNumber, byte[] data);
    }
}
