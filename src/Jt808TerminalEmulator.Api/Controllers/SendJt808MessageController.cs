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
        public async Task<IActionResult> IndexAsync(string phoneNumber)
        {
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                for (int index = 0; index < 5000; index++)
                {
                    await tcpClientFactory.CreateTcpClient($"{phoneNumber}{index}").ContinueWith(client => client.Result.ConnectAsync("127.0.0.1", 808).Wait(), TaskContinuationOptions.OnlyOnRanToCompletion);
                }
                stopwatch.Stop();
                return Ok(new { flag = true, data = tcpClientManager.GetTcpClients().Where(x => string.IsNullOrEmpty(x.ChannelId)), message = $"耗时：{stopwatch.ElapsedMilliseconds}毫秒" });
            }
            catch (Exception e)
            {
                return Ok(new { flag = false, message = e.Message });
            }
        }
    }
}
