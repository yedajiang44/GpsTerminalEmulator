using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jt808TerminalEmulator.Interface;
using Jt808TerminalEmulator.Model.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jt808TerminalEmulator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TerminalController : ControllerBase
    {
        readonly ITerminalService terminalService;

        public TerminalController(ITerminalService terminalService)
        {
            this.terminalService = terminalService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Random(int count = 1)
        {
            return Ok(new JsonResultDto { Flag = true, Data = await terminalService.AddRandom(count) });
        }
    }
}
