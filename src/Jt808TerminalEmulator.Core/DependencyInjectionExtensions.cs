using GpsPlatform.Jt808Protocol;
using Jt808TerminalEmulator.Core.Abstract;
using Jt808TerminalEmulator.Core.Netty;
using Jt808TerminalEmulator.Core.Netty.Codec;
using Jt808TerminalEmulator.Core.Netty.Handler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jt808TerminalEmulator.Core
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection UseJt808TerminalEmulator(this IServiceCollection services)
        {
            return services.AddSingleton<PackageConverter>()
                .AddSingleton<ITcpClientManager, TcpClientManager>()
                .AddSingleton<ISessionManager, SessionManager>()
                .AddSingleton<ITcpClientFactory, TcpClientFactory>()
                .AddScoped<ITcpClient, TcpClient>()
                .AddScoped<Jt808TcpHandler>()
                .AddScoped<Jt808Decoder>()
                .AddScoped<Jt808Encoder>();
        }
    }
}
