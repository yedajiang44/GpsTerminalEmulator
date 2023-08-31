using System.Linq.Dynamic.Core;
using Jt808TerminalEmulator.Core;
using Jt808TerminalEmulator.Core.Abstract;
using Jt808TerminalEmulator.Interface;
using Jt808TerminalEmulator.Model.Dtos;
using Jt808TerminalEmulator.Model.Enum;
using Jt808TerminalEmulator.Model.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Jt808TerminalEmulator.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BenchmarkController : ControllerBase
{
    private readonly ISessionManager sessionManager;
    private readonly ITcpClientFactory tcpClientFactory;
    private readonly ITerminalService terminalService;
    private readonly ILogger logger;

    public BenchmarkController(ITcpClientFactory tcpClientFactory, ITerminalService terminalService, ILogger<BenchmarkController> logger, ISessionManager sessionManager)
    {
        this.tcpClientFactory = tcpClientFactory;
        this.terminalService = terminalService;
        this.logger = logger;
        this.sessionManager = sessionManager;
    }

    [HttpGet]
    public async Task<IActionResult> IndexAsync(string ip, int port, string lineId, double speed, int interval, TaskType type = TaskType.Once)
    {
        foreach (var session in sessionManager.GetTcpClientSessions())
        {
            try
            {
                await session.Close();
            }
            catch (Exception e)
            {
                logger.LogError(e, "stop error: {message}", e.Message);
            }
        }
        var client = await tcpClientFactory.CreateTcpClient();
        var terminals = await terminalService.Query<TerminalFilter>();
        _ = terminals.ConvertAll(async x =>
        {
            try
            {
                var session = await client.ConnectAsync(ip, port, x.Sim);
                await session.StartTask(lineId, speed, interval, type);
            }
            catch (Exception e)
            {
                logger.LogError(e, "connect error: {message}", e.Message);
            }
        })
        .ToList();
        return Ok(new JsonResultDto<int>
        {
            Flag = true,
            Data = terminals.Count
        });
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> StopAsync()
    {
        foreach (var session in sessionManager.GetTcpClientSessions())
        {
            try
            {
                await session.Close();
            }
            catch (Exception e)
            {
                logger.LogError(e, "stop error: {message}", e.Message);
            }
        }
        return Ok(new JsonResultDto<bool> { Flag = true });
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Search([FromQuery] TerminalFilter filter)
    {
        //StartDateTime.ascend-LastActiveDateTime.desc
        var sort = filter.Sort?.Replace("-", ",").Replace('.', ' ').Replace("ascend", "").Replace("descend", "desc");
        var pageSize = filter.Size;
        var pageIndex = filter.Index;
        var terminals = await terminalService.Query(filter);
        var sessions = sessionManager.GetTcpClientSessions();
        var result = terminals.Select(x =>
        {
            var session = sessions.FirstOrDefault(item => item.PhoneNumber == x.Sim);
            return new BenchmarkListItemDto
            {
                SimNumber = x.Sim,
                Online = session != default,
                LicensePlate = x.LicensePlate,
                StartDateTime = session?.StartDateTime,
                LastActiveDateTime = session?.LastActiveDateTime,
                OnlineTime = session?.StartDateTime != null && session?.LastActiveDateTime != null ? TimeOnly.FromTimeSpan(session.LastActiveDateTime - session.StartDateTime) : TimeOnly.MinValue
            };
        });
        result = filter.OnlineState switch
        {
            OnlineStatus.Online => result.Where(x => x.Online),
            OnlineStatus.Offline => result.Where(x => !x.Online),
            _ => result
        };
        if (!string.IsNullOrWhiteSpace(sort))
        {
            result = result.AsQueryable()
            .OrderBy(sort);
        }

        return Ok(new PageResultDto<BenchmarkListItemDto>
        {
            List = result.Skip(pageSize * (pageIndex - 1)).Take(pageSize),
            Total = terminals.Count
        });
    }
}
