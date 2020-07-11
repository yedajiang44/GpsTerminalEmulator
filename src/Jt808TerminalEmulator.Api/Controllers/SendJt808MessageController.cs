﻿using System;
using System.Threading.Tasks;
using GpsPlatform.Jt808Protocol.Instruction;
using GpsPlatform.Jt808Protocol.PackageInfo;
using Jt808TerminalEmulator.Core;
using Microsoft.AspNetCore.Mvc;


namespace Jt808TerminalEmulator.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SendJt808MessageController : ControllerBase
    {
        private readonly ITcpClientFactory tcpClientFactory;

        public SendJt808MessageController(ITcpClientFactory tcpClientFactory)
        {
            this.tcpClientFactory = tcpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync(string phoneNumber)
        {
            try
            {
                var package = new Jt808PackageInfo { Header = new Header { PhoneNumber = phoneNumber }, Body = new Jt808_0x0002_Heartbeat() };
                var client = await tcpClientFactory.CreateTcpClient(package.Header.PhoneNumber);
                await client.ConnectAsync("127.0.0.1", 808);
                await client.Send(package);
                return Ok(new { flag = true, data = package });
            }
            catch (Exception e)
            {
                return Ok(new { flag = false, message = e.Message });
            }
        }
    }
}
