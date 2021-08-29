using GpsPlatform.Jt808Protocol;
using Jt808TerminalEmulator.Core.Abstract;
using Jt808TerminalEmulator.Core.Netty;
using Jt808TerminalEmulator.Core.Netty.Codec;
using Jt808TerminalEmulator.Core.Netty.Handler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Jt808TerminalEmulator.Core
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceProvider ServiceProvider { get; set; }
        public static IServiceCollection UseJt808TerminalEmulator(this IServiceCollection services, IConfigurationSection configurationKey)
        {
            return services.AddSingleton<PackageConverter>()
                .AddSingleton<ITcpClientManager, TcpClientManager>()
                .AddSingleton<ISessionManager, SessionManager>()
                .AddSingleton<ITcpClientFactory, TcpClientFactory>()
                .AddSingleton<IPackageConverter, DefaultIPackageConverter>()
                .AddSingleton<LineManager>()
                .AddSingleton<LocationInterpolation>()
                .AddSingleton<WebSocketSessionManager>()
                .AddScoped<ITcpClient, TcpClient>()
                .AddScoped<Jt808TcpHandler>()
                .AddScoped<WebSocketServerHandler>()
                .AddScoped<Jt808Decoder>()
                .AddScoped<Jt808Encoder>()
                .AddHostedService<WebSocketHost>()
                .Configure<GatewayConfiguration>(configurationKey);
        }
    }
}
