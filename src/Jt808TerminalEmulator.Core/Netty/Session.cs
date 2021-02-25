using System.Threading;
using DotNetty.Common.Concurrency;
using DotNetty.Transport.Channels;
using GpsPlatform.Jt808Protocol;
using GpsPlatform.Jt808Protocol.PackageInfo;
using Jt808TerminalEmulator.Core.Abstract;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GpsPlatform.Jt808Protocol.Instruction;
using GpsPlatform.Jt808Protocol.Instruction.LocationReportInformation.Basic;
using Jt808TerminalEmulator.Model.Dtos;
using Microsoft.Extensions.Logging;

namespace Jt808TerminalEmulator.Core.Netty
{
    internal class Session : ISession
    {
        ILogger logger;
        public IChannel Channel { get; set; }

        public string Id => Channel?.Id.AsLongText();

        public int nextLocaltionIndex { get; set; }

        public DateTime LastLocationDateTime { get; set; }

        public LocationDto LastLocation { get; set; }

        public string PhoneNumber { get; set; }

        public Task Send(Jt808PackageInfo data) => Channel.WriteAndFlushAsync(data);

        public Task Send(byte[] data) => Channel.WriteAndFlushAsync(data);

        private IPackageConverter packageConverter;

        private CancellationTokenSource cts;

        public Session()
        {
            packageConverter = Jt808TerminalEmulator.Core.DependencyInjectionExtensions.ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<IPackageConverter>();
            logger = Jt808TerminalEmulator.Core.DependencyInjectionExtensions.ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<ILogger<Session>>();
        }

        public Task<bool> StartTask(string lineId, double speed, int interval) => Task.Run<bool>(() =>
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();
            Task.Run(async () =>
            {
                var index = 0;
                while (!cts.IsCancellationRequested)
                {
                    logger.LogInformation($"当前索引{index}");
                    LastLocation = LineManager.GetNextLocaltion(lineId, LastLocation, speed, interval, ref index);
                    if (LastLocation == default) break;
                    this.nextLocaltionIndex = index;
                    await Send(new Jt808PackageInfo
                    {
                        Header = new Header { PhoneNumber = PhoneNumber },
                        Body = new Jt808_0x0200_LocationReport
                        {
                            AlarmSign = new AlarmSign(0),
                            Status = new Status(0),
                            Longitude = (int)(LastLocation.Logintude * 10e6),
                            Latitude = (int)(LastLocation.Latitude * 10e6)
                        }
                    });
                    await Task.Delay(TimeSpan.FromSeconds(interval));
                }
                nextLocaltionIndex = 0;
            });
            return true;
        });
        public Task<bool> StopTask() => Task.Run<bool>(() =>
        {
            cts.Cancel();
            return true;
        });
    }
    public interface ISession
    {
        public string Id { get; }
        public string PhoneNumber { get; set; }
        Task Send(Jt808PackageInfo data);
        Task Send(byte[] data);
        Task<bool> StartTask(string lineId, double speed, int interval);
        Task<bool> StopTask();
    }
}
