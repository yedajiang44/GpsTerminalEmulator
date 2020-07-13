using DotNetty.Transport.Channels;
using GpsPlatform.Jt808Protocol.PackageInfo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jt808TerminalEmulator.Core.Netty
{
    internal class Session : ISession
    {
        public IChannel Channel { get; set; }
        public string Id => Channel?.Id.AsLongText();

        public string PhoneNumber { get; set; }

        public Task Send(Jt808PackageInfo data) => Channel.WriteAndFlushAsync(data);

        public Task Send(byte[] data) => Channel.WriteAndFlushAsync(data);
    }
    public interface ISession
    {
        public string Id { get; }
        public string PhoneNumber { get; set; }
        Task Send(Jt808PackageInfo data);
        Task Send(byte[] data);
    }
}
