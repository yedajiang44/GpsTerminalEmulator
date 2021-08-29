using System.Threading;
using DotNetty.Transport.Channels;
using GpsPlatform.Jt808Protocol.PackageInfo;
using Jt808TerminalEmulator.Core.Abstract;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using GpsPlatform.Jt808Protocol.Instruction;
using GpsPlatform.Jt808Protocol.Instruction.LocationReportInformation.Basic;
using Jt808TerminalEmulator.Model.Dtos;
using Microsoft.Extensions.Logging;

namespace Jt808TerminalEmulator.Core.Netty
{
    public class WebSocketSession : IDisposable
    {
        public string Id => Channel.Id.AsShortText();
        public IChannel Channel { get; private set; }
        public DateTime LastActiveTime { get; set; }
        public DateTime StartTime { get; set; }
        public WebSocketSession(IChannel channel)
        {
            Channel = channel;
        }
        public Task Send(byte[] data) => Channel.WriteAndFlushAsync(data);
        public Task Send(string data) => Channel.WriteAndFlushAsync(data);

        public void Dispose()
        {
            Channel.DisconnectAsync();
        }
    }

    internal class Session : ISession
    {
        ILogger logger;
        public IChannel Channel { get; set; }

        public string Id => Channel?.Id.AsLongText();

        public int nextLocaltionIndex { get; set; }

        public DateTime LastLocationDateTime { get; set; }

        public LocationDto LastLocation { get; set; }

        public Status Status { get; set; } = new Status(0);

        public AlarmSign AlarmSign { get; set; } = new AlarmSign(0);

        public double Speed { get; set; }

        public int Interval { get; set; }

        public string PhoneNumber { get; set; }

        public Task Send(Jt808PackageInfo data) => Channel.WriteAndFlushAsync(data);

        public Task Send(byte[] data) => Channel.WriteAndFlushAsync(data);

        private IPackageConverter packageConverter;

        private CancellationTokenSource cts;

        private LineManager lineManager;

        public Session(IServiceProvider serviceProvider)
        {
            packageConverter = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IPackageConverter>();
            logger = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ILogger<Session>>();
            lineManager = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<LineManager>();
        }

        public Task<bool> StartTask(string lineId, double speed, int interval) => Task.Run<bool>(() =>
        {
            Speed = speed;
            Status.ACC = true;
            Status.Locate = true;
            Interval = interval;
            cts?.Cancel();
            cts = new CancellationTokenSource();
            Task.Run(async () =>
            {
                var index = 0;
                while (!cts.IsCancellationRequested)
                {
                    logger.LogInformation($"当前索引{index}");
                    LastLocation = lineManager.GetNextLocaltion(lineId, LastLocation, Speed, interval, ref index);
                    if (LastLocation == default) break;
                    this.nextLocaltionIndex = index;
                    await Send(new Jt808PackageInfo
                    {
                        Header = new Header { PhoneNumber = PhoneNumber },
                        Body = new Jt808_0x0200_LocationReport
                        {
                            AlarmSign = AlarmSign,
                            Status = Status,
                            Speed = (ushort)(speed * 10),
                            Longitude = (int)(LastLocation.Logintude * 10e5),
                            Latitude = (int)(LastLocation.Latitude * 10e5),
                            DateTime = DateTime.Now,
                        }
                    });
                    await Task.Delay(TimeSpan.FromSeconds(Interval));
                }
                Speed = 0;
                Status.ACC = false;
                Status.Locate = false;
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
        string Id { get; }
        string PhoneNumber { get; set; }

        Status Status { get; set; }

        AlarmSign AlarmSign { get; set; }

        double Speed { get; set; }

        int Interval { get; set; }
        Task Send(Jt808PackageInfo data);
        Task Send(byte[] data);
        Task<bool> StartTask(string lineId, double speed, int interval);
        Task<bool> StopTask();
    }
}
