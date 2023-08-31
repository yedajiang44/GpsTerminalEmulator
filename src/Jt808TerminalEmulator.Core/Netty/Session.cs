using DotNetty.Transport.Channels;
using GpsPlatform.Jt808Protocol.Instruction;
using GpsPlatform.Jt808Protocol.PackageInfo;
using Jt808TerminalEmulator.Model.Dtos;
using Jt808TerminalEmulator.Model.Enum;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static GpsPlatform.Jt808Protocol.Instruction.Jt808_0x0200_LocationReport;

namespace Jt808TerminalEmulator.Core.Netty;

internal class WebSocketSession : IWebSocketSession
{
    public string Id => Channel.Id.AsShortText();
    IChannel Channel { get; }
    public DateTime LastActiveTime { get; set; }
    public DateTime StartTime { get; }

    public DateTime StartDateTime { get; } = DateTime.Now;

    public DateTime LastActiveDateTime { get; set; }

    public WebSocketSession(IChannel channel)
    {
        Channel = channel;
        StartTime = DateTime.Now;
    }
    public Task Send(byte[] data)
    {
        LastActiveDateTime = DateTime.Now;
        return Channel.WriteAndFlushAsync(data);
    }
    public Task Send(string data)
    {
        LastActiveDateTime = DateTime.Now;
        return Channel.WriteAndFlushAsync(data);
    }

    public async Task Close()
    {
        await Channel.CloseAsync();
    }
}

internal class TcpClientSession : ITcpClientSession
{
    private readonly ILogger logger;
    public IChannel Channel { get; set; }

    public string Id => Channel?.Id.AsLongText();

    public bool Logged { get; set; }

    public Jt808_0x0200_LocationReport LastLocation { get; set; }

    public Status Status { get; set; } = new Status(1);

    public Alarm Alarm { get; set; } = new Alarm(0);

    public double Speed { get; set; }

    public int Interval { get; set; }

    public string PhoneNumber { get; set; }

    public DateTime StartDateTime { get; } = DateTime.Now;

    public DateTime LastActiveDateTime { get; set; }

    public Task Send(Jt808PackageInfo data)
    {
        LastActiveDateTime = DateTime.Now;
        return Channel.WriteAndFlushAsync(data);
    }

    public Task Send(byte[] data)
    {
        LastActiveDateTime = DateTime.Now;
        return Channel.WriteAndFlushAsync(data);
    }

    private CancellationTokenSource cts;

    private readonly LineManager lineManager;

    public TcpClientSession(IServiceProvider serviceProvider, IChannel channel, string phoneNumber)
    {
        var provider = serviceProvider.CreateScope().ServiceProvider;
        logger = provider.GetRequiredService<ILogger<TcpClientSession>>();
        lineManager = provider.GetRequiredService<LineManager>();
        Channel = channel;
        PhoneNumber = phoneNumber;

        Send(new Jt808PackageInfo
        {
            Header = new Header { PhoneNumber = PhoneNumber },
            Body = new Jt808_0x0100_Register
            {
                ManufacturerId = "",
                TerminalModel = "",
                TerminalId = phoneNumber,
                //TODO: 查询并赋值车牌号
                VehicleIdentification = ""
            }
        });
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
                if (!Logged)
                {
                    logger.LogInformation("正在等待登录鉴权.");
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    continue;
                }
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
                    VehicleAlarm = Alarm,
                    VehicleStatus = Status,
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
                await Task.Delay(TimeSpan.FromSeconds(Interval), cts.Token);
            }
            Speed = 0;
            Status.ACC = false;
            Status.Locate = false;
        });
        return true;
    });

    public async Task Close()
    {
        if (!cts.IsCancellationRequested) cts.Cancel();
        await Channel.CloseAsync();
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

    Alarm Alarm { get; set; }

    double Speed { get; set; }
    Jt808_0x0200_LocationReport LastLocation { get; }

    int Interval { get; set; }
    Task Send(Jt808PackageInfo data);
    Task Send(byte[] data);
    Task<bool> StartTask(string lineId, double speed, int interval, TaskType type = TaskType.Once);
}
public interface ISession
{
    string Id { get; }
    DateTime StartDateTime { get; }
    DateTime LastActiveDateTime { get; set; }
    Task Close();
}