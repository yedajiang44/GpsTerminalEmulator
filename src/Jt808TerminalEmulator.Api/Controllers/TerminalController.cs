using Jt808TerminalEmulator.Interface;
using Jt808TerminalEmulator.Model.Dtos;
using Jt808TerminalEmulator.Model.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jt808TerminalEmulator.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TerminalController : ControllerBase
    {
        readonly ITerminalService terminalService;

        public TerminalController(ITerminalService terminalService)
        {
            this.terminalService = terminalService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Find(string id)
        {
            return Ok(new JsonResultDto<TerminalDto> { Data = await terminalService.Find(x => x.Id == id) });
        }

        [HttpGet]
        public async Task<IActionResult> FindAll()
        {
            return Ok(new JsonResultDto<IList<TerminalDto>> { Data = await terminalService.Query<TerminalFilter>() });
        }

        [HttpGet("[action]/{count?}")]
        public async Task<IActionResult> Random(int count = 1)
        {
            return Ok(new JsonResultDto<int> { Data = await terminalService.AddRandom(count) });
        }

        [HttpPost]
        public async Task<IActionResult> Add(TerminalDto dto)
        {
            var result = await terminalService.Add(dto);
            return Ok(new JsonResultDto<string>
            {
                Data = result,
                Message = string.IsNullOrEmpty(result) ? null : "操作失败"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await terminalService.Delete(x => x.Id == id) > 0;
            return Ok(new JsonResultDto<bool>
            {
                Data = result,
                Message = result ? null : "操作失败"
            });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] string[] ids)
        {
            var result = await terminalService.Delete(x => ids.Contains(x.Id)) > 0;
            return Ok(new JsonResultDto<bool>
            {
                Data = result,
                Message = result ? null : "操作失败"
            });
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteAll()
        {
            var result = await terminalService.Delete(x => true) > 0;
            return Ok(new JsonResultDto<bool>
            {
                Data = result,
                Message = result ? null : "操作失败"
            });
        }

        [HttpPut]
        public async Task<IActionResult> Update(TerminalDto dto)
        {
            await terminalService.Update(dto);
            return Ok(new JsonResultDto<TerminalDto>
            {
                Data = dto
            });
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Search([FromQuery] TerminalFilter filter)
        {
            return Ok(await terminalService.QueryWithPage(filter));
        }
    }
}
