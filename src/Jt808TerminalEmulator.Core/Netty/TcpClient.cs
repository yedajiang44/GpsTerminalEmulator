using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using GpsPlatform.Jt808Protocol.PackageInfo;
using Jt808TerminalEmulator.Core.Netty.Codec;
using Jt808TerminalEmulator.Core.Netty.Handler;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Jt808TerminalEmulator.Core.Netty
{
    internal class TcpClient : ITcpClient
    {
        readonly MultithreadEventLoopGroup eventLoopGroup;
        readonly Bootstrap bootstrap;
        private IChannel channel;

        public TcpClient(IServiceProvider serviceProvider)
        {
            eventLoopGroup = new MultithreadEventLoopGroup();
            bootstrap = new Bootstrap().Group(eventLoopGroup)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    var scope = serviceProvider.CreateScope().ServiceProvider;
                    IChannelPipeline pipeline = channel.Pipeline;
                    pipeline.AddLast(scope.GetRequiredService<Jt808Encoder>());
                    pipeline.AddLast(scope.GetRequiredService<Jt808Decoder>());
                    pipeline.AddLast(scope.GetRequiredService<Jt808TcpHandler>());
                }));
        }

        public async Task<bool> ConnectAsync(string ip, int port)
        {
            return await bootstrap.ConnectAsync(ip, port).ContinueWith(x => channel = x.Result) != default(IChannel);
        }

        public Task Send(Jt808PackageInfo data, Action action = default) => channel.WriteAndFlushAsync(data).ContinueWith(x => { if (action != default) { action(); } });
        public Task Send(byte[] data, Action action = default) => channel.WriteAndFlushAsync(data).ContinueWith(x => { if (action != default) { action(); } });
    }

    public interface ITcpClient
    {
        Task<bool> ConnectAsync(string ip, int port);
        Task Send(Jt808PackageInfo data, Action action = default);
        Task Send(byte[] data, Action action = default);
    }
}
