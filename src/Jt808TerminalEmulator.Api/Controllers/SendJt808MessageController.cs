using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GpsPlatform.Jt808Protocol.Instruction;
using GpsPlatform.Jt808Protocol.PackageInfo;
using Jt808TerminalEmulator.Core;
using Jt808TerminalEmulator.Core.Abstract;
using Microsoft.AspNetCore.Mvc;


namespace Jt808TerminalEmulator.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SendJt808MessageController : ControllerBase
    {
        private readonly ITcpClientFactory tcpClientFactory;
        private readonly ITcpClientManager tcpClientManager;

        public SendJt808MessageController(ITcpClientFactory tcpClientFactory, ITcpClientManager tcpClientManager)
        {
            this.tcpClientFactory = tcpClientFactory;
            this.tcpClientManager = tcpClientManager;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync(string phoneNumber = "13800138000")
        {
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var client = await tcpClientFactory.CreateTcpClient($"{phoneNumber}");
                Parallel.For(0, 5000, index =>
                {
                    client.ConnectAsync("127.0.0.1", 2012, $"{phoneNumber}{index}");
                });
                stopwatch.Stop();
                return Ok(new { flag = true, data = tcpClientManager.GetTcpClients().SelectMany(x => x.Sesions().Result), message = $"耗时：{stopwatch.ElapsedMilliseconds}毫秒" });
            }
            catch (Exception e)
            {
                return Ok(new { flag = false, message = e.Message });
            }
        }
    }
}
