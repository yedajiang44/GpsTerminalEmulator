using System.ComponentModel;
using GpsPlatform.Infrastructure.Extentions;
using GpsPlatform.Jt808Protocol.Instruction;
using Jt808TerminalEmulator.Core;
using Jt808TerminalEmulator.Core.Netty;
using Jt808TerminalEmulator.Interface;
using Jt808TerminalEmulator.Model.Dtos;
using Jt808TerminalEmulator.Model.Enum;
using Jt808TerminalEmulator.Model.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jt808TerminalEmulator.Api.Controllers
{
    [Authorize]
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
            var result = await currentservice.Add(dto);
            return Ok(new JsonResultDto<string>
            {
                Data = result,
                Message = string.IsNullOrEmpty(result) ? null : "操作失败"
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

        [AllowAnonymous]
        [HttpGet("type/[action]/{keyword?}")]
        public IActionResult TypeSearch(string keyword)
        {
            return Ok(Enum.GetValues<TaskType>().Select(x => new { value = (int)x, description = x.ToDescription() }).Where(x => string.IsNullOrEmpty(keyword) || x.description.Contains(keyword)));
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> Start([FromRoute] string id)
        {
            var task = await currentservice.Find(x => x.Id == id);
            var line = await lineService.Find(x => x.Id == task.LineId);
            var client = await tcpClientFactory.CreateTcpClient();
            var session = await client.ConnectAsync(task.Ip, task.Port, task.SimNumber);
            var result = await session.StartTask(line.Id, task.Speed, task.Interval);
            return Ok(new JsonResultDto<bool> { Flag = result, Data = result });
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> Stop([FromRoute] string id)
        {
            var task = await currentservice.Find(x => x.Id == id);
            var client = await tcpClientFactory.CreateTcpClient();
            var session = await client.GetSession(task.SimNumber);
            await session?.Close();
            return Ok(new JsonResultDto<bool> { Flag = true, Data = true });
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> Location([FromRoute] string id)
        {
            var result = new JsonResultDto<Jt808_0x0200_LocationReport> { Flag = false, Message = "未找到相关链路，请确认任务是否运行中" };
            var task = await currentservice.Find(x => x.Id == id);
            var client = await tcpClientFactory.CreateTcpClient();
            var session = await client.GetSession(task.SimNumber);
            if (session is ITcpClientSession tcpClientSession)
            {
                result.Flag = true;
                result.Data = session.LastLocation;
                result.Message = result.Data != null ? null : "找倒链路，但未找到最后定位点";
            }
            return Ok(result);
        }
    }
}
