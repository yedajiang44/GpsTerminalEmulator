using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GpsPlatform.Jt808Protocol.Instruction;
using GpsPlatform.Jt808Protocol.PackageInfo;
using Jt808TerminalEmulator.Core;
using Jt808TerminalEmulator.Core.Abstract;
using Jt808TerminalEmulator.Interface;
using Jt808TerminalEmulator.Model.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jt808TerminalEmulator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SendJt808MessageController : ControllerBase
    {
        private readonly ITcpClientFactory tcpClientFactory;
        private readonly ITcpClientManager tcpClientManager;
        private readonly ITerminalService terminalService;
        private readonly ILogger logger;

        public SendJt808MessageController(ITcpClientFactory tcpClientFactory, ITcpClientManager tcpClientManager, ITerminalService terminalService, ILogger<SendJt808MessageController> logger)
        {
            this.tcpClientFactory = tcpClientFactory;
            this.tcpClientManager = tcpClientManager;
            this.terminalService = terminalService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync(string ip, int port)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var client = await tcpClientFactory.CreateTcpClient();
            Parallel.ForEach(await terminalService.FindAll(), x => client.ConnectAsync(ip, port, x.Sim));
            return Ok(new JsonResultDto
            {
                Flag = true,
                Data = tcpClientManager.GetTcpClients().SelectMany(x => x.Sesions().Result),
                Message = $"耗时：{stopwatch.ElapsedMilliseconds}毫秒"
            });
        }
    }
}
