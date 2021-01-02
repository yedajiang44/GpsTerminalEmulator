using System;
using System.Linq;
using System.Threading.Tasks;
using Jt808TerminalEmulator.Interface;
using Jt808TerminalEmulator.Model.Dtos;
using Jt808TerminalEmulator.Model.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Jt808TerminalEmulator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        readonly ITaskService currentservice;
        public TaskController(ITaskService taskservice)
        {
            this.currentservice = taskservice;
        }

        [HttpPost]
        public async Task<IActionResult> Add(TaskDto dto)
        {
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
    }
}
