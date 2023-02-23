using System.Diagnostics;
using Jt808TerminalEmulator.Core;
using Jt808TerminalEmulator.Core.Abstract;
using Jt808TerminalEmulator.Interface;
using Jt808TerminalEmulator.Model.Dtos;
using Jt808TerminalEmulator.Model.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jt808TerminalEmulator.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SendJt808MessageController : ControllerBase
    {
        private readonly ITcpClientFactory tcpClientFactory;
        private readonly ITerminalService terminalService;
        private readonly ILogger logger;

        public SendJt808MessageController(ITcpClientFactory tcpClientFactory, ITerminalService terminalService, ILogger<SendJt808MessageController> logger)
        {
            this.tcpClientFactory = tcpClientFactory;
            this.terminalService = terminalService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync(string ip, int port)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var client = await tcpClientFactory.CreateTcpClient();
            var terminals = await terminalService.Query<TerminalFilter>();
            _ = terminals.ConvertAll(x => client.ConnectAsync(ip, port, x.Sim)
            .ContinueWith(x => logger.LogError(x.Exception, "connect error: {message}", x.Exception.Message), TaskContinuationOptions.NotOnRanToCompletion)
            .ConfigureAwait(false))
            .ToList();
            return Ok(new JsonResultDto<int>
            {
                Flag = true,
                Data = terminals.Count,
                Message = $"耗时：{stopwatch.ElapsedMilliseconds}毫秒"
            });
        }
    }
}
