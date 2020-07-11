using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Handlers.Timeout;
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
        public string PhoneNumber { get; set; }

        public string ChannelId => channel?.Id.AsLongText();

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
                    pipeline.AddLast(new IdleStateHandler(300, 5, 30));
                    pipeline.AddLast(new DelimiterBasedFrameDecoder(1024, Unpooled.CopiedBuffer(new byte[] { 0x7e }), Unpooled.CopiedBuffer(new byte[] { 0x7e })));
                    pipeline.AddLast(scope.GetRequiredService<Jt808Encoder>());
                    pipeline.AddLast(scope.GetRequiredService<Jt808Decoder>());
                    pipeline.AddLast(scope.GetRequiredService<Jt808TcpHandler>());
                }));
        }


        public async Task<bool> ConnectAsync(string ip, int port)
        {
            if (channel?.Open == true) return true;
            return await bootstrap.ConnectAsync(ip, port).ContinueWith(x => channel = x.Result) != default(IChannel);
        }

        public Task Send(Jt808PackageInfo data, Action action = default) => channel.WriteAndFlushAsync(data).ContinueWith(x => { if (action != default) { action(); } });
        public Task Send(byte[] data, Action action = default) => channel.WriteAndFlushAsync(data).ContinueWith(x => { if (action != default) { action(); } });
    }

    public interface ITcpClient
    {
        public string ChannelId { get; }
        public string PhoneNumber { get; set; }
        Task<bool> ConnectAsync(string ip, int port);
        Task Send(Jt808PackageInfo data, Action action = default);
        Task Send(byte[] data, Action action = default);
    }
}
