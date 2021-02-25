using System.Net;
using System;
using System.Linq;
using System.Threading.Tasks;
using Jt808TerminalEmulator.Interface;
using Jt808TerminalEmulator.Model.Dtos;
using Jt808TerminalEmulator.Model.Filters;
using Microsoft.AspNetCore.Mvc;
using Jt808TerminalEmulator.Core;
using System.Collections.Generic;

namespace Jt808TerminalEmulator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        readonly ITaskService currentservice;
        readonly ITerminalService terminalService;
        readonly ILineService lineService;
        readonly ITcpClientFactory tcpClientFactory;
        public TaskController(ITaskService taskservice, ILineService lineService, ITcpClientFactory tcpClientFactory, ITerminalService terminalService)
        {
            this.currentservice = taskservice;
            this.lineService = lineService;
            this.tcpClientFactory = tcpClientFactory;
            this.terminalService = terminalService;
        }

        [HttpPost]
        public async Task<IActionResult> Add(TaskDto dto)
        {
            var terminal = await terminalService.Find(x => x.Sim == dto.SimNumber);
            dto.Terminals = new List<TerminalDto> { new TerminalDto { Id = terminal.Id } };
            var result = await currentservice.Add(dto) > 0;
            return Ok(new JsonResultDto<bool>
            {
                Data = result,
                Message = result ? null : "操作失败"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await currentservice.Delete(x => x.Id == id) > 0;
            return Ok(new JsonResultDto<bool>
            {
                Data = result,
                Message = result ? null : "操作失败"
            });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] string[] ids)
        {
            var result = await currentservice.Delete(x => ids.Contains(x.Id)) > 0;
            return Ok(new JsonResultDto<bool>
            {
                Data = result,
                Message = result ? null : "操作失败"
            });
        }

        [HttpPut]
        public async Task<IActionResult> Update(TaskDto dto)
        {
            dto.Terminals = null;
            await currentservice.Update(dto);
            return Ok(new JsonResultDto<TaskDto>
            {
                Data = dto
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Find(string id)
        {
            return Ok(new JsonResultDto<TaskDto> { Data = await currentservice.Find(x => x.Id == id) });
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Search([FromQuery] TaskFilter filter)
        {
            return Ok(await currentservice.QueryWithPage(filter));
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> Start([FromRoute] string id)// TODO 使用202异步处理
        {
            var task = await currentservice.Find(x => x.Id == id);
            var line = await lineService.Find(x => x.Id == task.LineId);
            var client = await tcpClientFactory.CreateTcpClient();
            var session = await client.ConnectAsync(task.Ip, task.Port, task.SimNumber);
            var result = await session.StartTask(line.Id, task.Speed, task.Interval);
            return Ok(new JsonResultDto<bool> { Flag = result, Data = result });
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> Stop([FromRoute] string id)// TODO 使用202异步处理
        {
            var task = await currentservice.Find(x => x.Id == id);
            var client = await tcpClientFactory.CreateTcpClient();
            var session = await client.GetSession(task.SimNumber);
            var result = await session.StopTask();
            return Ok(new JsonResultDto<bool> { Flag = result, Data = result });
        }
    }
}
