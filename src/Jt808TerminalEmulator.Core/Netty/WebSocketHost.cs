using System;
using System.Net;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Codecs.Http;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Libuv;
using Jt808TerminalEmulator.Core.Abstract;
using Jt808TerminalEmulator.Core.Netty.Handler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jt808TerminalEmulator.Core.Netty
{
    public class WebSocketHost : IHostedService
    {
        readonly ILogger logger;
        IEventLoopGroup bossGroup;
        IEventLoopGroup workGroup;
        readonly IServiceProvider serviceProvider;
        readonly GatewayConfiguration configuration;

        public WebSocketHost(ILogger<WebSocketHost> logger, IOptions<GatewayConfiguration> options, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            configuration = options.Value;
            this.serviceProvider = serviceProvider;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (configuration.WebSocketPort <= 0) return Task.CompletedTask;
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
            }
            var dispatcher = new DispatcherEventLoopGroup();
            bossGroup = dispatcher;
            workGroup = new WorkerEventLoopGroup(dispatcher);

            var bootstrap = new ServerBootstrap();
            bootstrap.Group(bossGroup, workGroup);
            bootstrap.Channel<TcpServerChannel>();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                bootstrap
                    .Option(ChannelOption.SoReuseport, true)
                    .ChildOption(ChannelOption.SoReuseaddr, true);
            }
            bootstrap
                .Option(ChannelOption.SoBacklog, 8192)
                .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    using var scope = serviceProvider.CreateScope();
                    IChannelPipeline pipeline = channel.Pipeline;
                    pipeline.AddLast(new IdleStateHandler(configuration.ReaderIdleTimeSeconds, configuration.WriterIdleTimeSeconds, configuration.AllIdleTimeSeconds));
                    pipeline.AddLast(new HttpServerCodec());
                    pipeline.AddLast(new HttpObjectAggregator(65536));
                    pipeline.AddLast(scope.ServiceProvider.GetRequiredService<WebSocketServerHandler>());
                }));
            logger.LogInformation($"websocket server start at {IPAddress.Any}:{configuration.WebSocketPort}.");
            return bootstrap.BindAsync(configuration.WebSocketPort);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (configuration.WebSocketPort <= 0) return Task.CompletedTask;
            var quietPeriod = configuration.QuietPeriodTimeSpan;
            var shutdownTimeout = configuration.ShutdownTimeoutTimeSpan;
            return Task.WhenAll(bossGroup.ShutdownGracefullyAsync(quietPeriod, shutdownTimeout), workGroup.ShutdownGracefullyAsync(quietPeriod, shutdownTimeout));
        }
    }
}