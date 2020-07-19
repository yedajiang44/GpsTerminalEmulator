using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Jt808TerminalEmulator.Core;
using Jt808TerminalEmulator.Core.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jt808TerminalEmulator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly ITcpClientManager tcpClientManager;

        public SessionController(ITcpClientManager tcpClientManager)
        {
            this.tcpClientManager = tcpClientManager;
        }

        [HttpGet]
        public IActionResult IndexAsync()
        {
            try
            {
                return Ok(new { flag = true, data = tcpClientManager.GetTcpClients().SelectMany(x => x.Sesions().Result) });
            }
            catch (Exception e)
            {
                return Ok(new { flag = false, message = e.Message });
            }
        }
    }
}