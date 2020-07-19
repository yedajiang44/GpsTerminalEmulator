using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GpsPlatform.Jt808Protocol.Instruction;
using GpsPlatform.Jt808Protocol.PackageInfo;
using Jt808TerminalEmulator.Core;
using Jt808TerminalEmulator.Core.Abstract;
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
        private readonly ILogger logger;

        public SendJt808MessageController(ITcpClientFactory tcpClientFactory, ITcpClientManager tcpClientManager, ILogger<SendJt808MessageController> logger)
        {
            this.tcpClientFactory = tcpClientFactory;
            this.tcpClientManager = tcpClientManager;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync(string phoneNumber = "13800138000")
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var client = await tcpClientFactory.CreateTcpClient($"{phoneNumber}");
            for (int index = 0; index < 5000; index++)
            {
                try
                {
                    await client.ConnectAsync("127.0.0.1", 2012, $"{index}");
                }
                catch (Exception e)
                {
                    logger.LogError(e, "创建连接发生异常");
                }
            }
            return Ok(new JsonResultDto { Flag = true, Data = tcpClientManager.GetTcpClients().SelectMany(x => x.Sesions().Result), Message = $"耗时：{stopwatch.ElapsedMilliseconds}毫秒" });
        }
    }
}
