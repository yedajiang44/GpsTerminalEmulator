using Jt808TerminalEmulator.Core.Netty;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jt808TerminalEmulator.Core
{
    internal class TcpClientFactory : ITcpClientFactory
    {
        readonly IServiceProvider serviceProvider;
        public TcpClientFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public Task<ITcpClient> CreateTcpClient()
        {
            return Task.FromResult(serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ITcpClient>());
        }
    }
    public interface ITcpClientFactory
    {
        Task<ITcpClient> CreateTcpClient();
    }
}
