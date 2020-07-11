using Jt808TerminalEmulator.Core.Abstract;
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
        readonly ITcpClientManager tcpClientManager;

        public TcpClientFactory(IServiceProvider serviceProvider, ITcpClientManager tcpClientManager)
        {
            this.serviceProvider = serviceProvider;
            this.tcpClientManager = tcpClientManager;
        }

        public Task<ITcpClient> CreateTcpClient(string phoneNumber, bool fromCache = true, bool addManager = true)
        {
            if (string.IsNullOrEmpty(phoneNumber)) throw new NullReferenceException($"the {nameof(phoneNumber)} is null or empty");
            var client = fromCache ? tcpClientManager.GetTcpClient(phoneNumber) : default;
            if (client == default)
            {
                client = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ITcpClient>();
                client.PhoneNumber = phoneNumber;
                if (addManager)
                    tcpClientManager.Add(client);
            }
            return Task.FromResult(client);
        }
    }
    public interface ITcpClientFactory
    {
        Task<ITcpClient> CreateTcpClient(string phoneNumber, bool fromCache = true, bool addManager = true);
    }
}
