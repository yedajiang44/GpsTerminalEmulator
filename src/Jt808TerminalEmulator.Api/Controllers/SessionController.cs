using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Jt808TerminalEmulator.Core;
using Jt808TerminalEmulator.Core.Abstract;
using Jt808TerminalEmulator.Core.Netty;
using Jt808TerminalEmulator.Interface;
using Jt808TerminalEmulator.Model.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Jt808TerminalEmulator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly ITcpClientManager tcpClientManager;
        private readonly ITerminalService terminalService;

        public SessionController(ITcpClientManager tcpClientManager, ITerminalService terminalService)
        {
            this.tcpClientManager = tcpClientManager;
            this.terminalService = terminalService;
        }

        [HttpGet]
        public IActionResult IndexAsync()
        {
            try
            {
                return Ok(new JsonResultDto<IEnumerable<ISession>>
                {
                    Flag = true,
                    Data = tcpClientManager.GetTcpClients().SelectMany(x => x.Sesions().Result)
                });
            }
            catch (Exception e)
            {
                return Ok(new JsonResultDto<IEnumerable<ISession>>
                {
                    Message = e.Message
                });
            }
        }
        [HttpGet("[action]")]
        public IActionResult Count()
        {
            try
            {
                return Ok(new JsonResultDto<long>
                {
                    Flag = true,
                    Data = tcpClientManager.GetTcpClients().SelectMany(x => x.Sesions().Result).Count()
                });
            }
            catch (Exception e)
            {
                return Ok(new JsonResultDto<long>
                {
                    Message = e.Message
                });
            }
        }
    }
}