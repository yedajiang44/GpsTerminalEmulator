using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using GpsPlatform.Jt808Protocol.Instruction;
using GpsPlatform.Jt808Protocol.Instruction.LocationReportInformation.Basic;
using GpsPlatform.Jt808Protocol.PackageInfo;
using Jt808TerminalEmulator.Model.Dtos;
using Jt808TerminalEmulator.Model.Enum;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Jt808TerminalEmulator.Core.Netty
{
    internal class WebSocketSession : IWebSocketSession
    {
        public string Id => Channel.Id.AsShortText();
        IChannel Channel { get; }
        public DateTime LastActiveTime { get; set; }
        public DateTime StartTime { get; }
        public WebSocketSession(IChannel channel)
        {
            Channel = channel;
            StartTime = DateTime.Now;
        }
        public Task Send(byte[] data) => Channel.WriteAndFlushAsync(data);
        public Task Send(string data) => Channel.WriteAndFlushAsync(data);

        public void Dispose()
        {
            Channel.CloseAsync();
        }
    }

    internal class TcpClientSession : ITcpClientSession
    {
        private readonly ILogger logger;
        public IChannel Channel { get; set; }

        public string Id => Channel?.Id.AsLongText();

        public Jt808_0x0200_LocationReport LastLocation { get; set; }

        public Status Status { get; set; } = new Status(0);

        public AlarmSign AlarmSign { get; set; } = new AlarmSign(0);

        public double Speed { get; set; }

        public int Interval { get; set; }

        public string PhoneNumber { get; set; }

        public Task Send(Jt808PackageInfo data) => Channel.WriteAndFlushAsync(data);

        public Task Send(byte[] data) => Channel.WriteAndFlushAsync(data);

        private CancellationTokenSource cts;

        private readonly LineManager lineManager;

        public TcpClientSession(IServiceProvider serviceProvider)
        {
            logger = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ILogger<TcpClientSession>>();
            lineManager = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<LineManager>();
        }

        public Task<bool> StartTask(string lineId, double speed, int interval, TaskType type) => Task.Run(() =>
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
                var reverse = false;
                var location = new LocationDto();
                while (!cts.IsCancellationRequested)
                {
                    logger.LogInformation($"当前索引{index}");
                    location = lineManager.GetNextLocation(lineId, location, Speed, interval, ref index, reverse);
                    if (location == default)
                    {
                        index = 0;
                        if (type == TaskType.LoopBack) reverse = !reverse;
                        if (type == TaskType.Once)
                            break;
                        else
                            continue;
                    }
                    LastLocation = new Jt808_0x0200_LocationReport
                    {
                        AlarmSign = AlarmSign,
                        Status = Status,
                        Speed = (ushort)(speed * 10),
                        Longitude = (int)(location.Logintude * 10e5),
                        Latitude = (int)(location.Latitude * 10e5),
                        DateTime = DateTime.Now,
                    };
                    await Send(new Jt808PackageInfo
                    {
                        Header = new Header { PhoneNumber = PhoneNumber },
                        Body = LastLocation
                    });
                    await Task.Delay(TimeSpan.FromSeconds(Interval));
                }
                Speed = 0;
                Status.ACC = false;
                Status.Locate = false;
            });
            return true;
        });
        public Task<bool> StopTask() => Task.Run(() =>
        {
            cts.Cancel();
            return true;
        });

        public void Dispose()
        {
            if (!cts.IsCancellationRequested) cts.Cancel();
            Channel.DisconnectAsync();
        }
    }
    public interface IWebSocketSession : ISession
    {
        DateTime StartTime { get; }
        DateTime LastActiveTime { get; set; }
        Task Send(byte[] data);
        Task Send(string data);
    }
    public interface ITcpClientSession : ISession
    {
        string PhoneNumber { get; set; }

        Status Status { get; set; }

        AlarmSign AlarmSign { get; set; }

        double Speed { get; set; }

        int Interval { get; set; }
        Task Send(Jt808PackageInfo data);
        Task Send(byte[] data);
        Task<bool> StartTask(string lineId, double speed, int interval, TaskType type = TaskType.Once);
        Task<bool> StopTask();
    }
    public interface ISession : IDisposable
    {
        string Id { get; }
    }
}
