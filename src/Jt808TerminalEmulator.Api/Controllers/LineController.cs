using Jt808TerminalEmulator.Core.Netty;
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
    public class LineController : ControllerBase
    {
        readonly ILineService lineService;
        readonly LineManager lineManager;

        public LineController(ILineService lineService, LineManager lineManager)
        {
            this.lineService = lineService;
            this.lineManager = lineManager;
        }

        [HttpPost]
        public async Task<IActionResult> Add(LineDto dto)
        {
            var result = await lineService.Add(dto);
            lineManager.Add(dto);
            return Ok(new JsonResultDto<string>
            {
                Data = result,
                Message = string.IsNullOrEmpty(result) ? null : "操作失败"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await lineService.Delete(x => x.Id == id) > 0;
            lineManager.Remove(id);
            return Ok(new JsonResultDto<bool>
            {
                Data = result,
                Message = result ? null : "操作失败"
            });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] string[] ids)
        {
            var result = await lineService.Delete(x => ids.Contains(x.Id)) > 0;
            return Ok(new JsonResultDto<bool>
            {
                Data = result,
                Message = result ? null : "操作失败"
            });
        }

        [HttpPut]
        public async Task<IActionResult> Update(LineDto dto)
        {
            await lineService.Update(dto);
            lineManager.Add(dto);
            return Ok(new JsonResultDto<LineDto>
            {
                Data = dto
            });
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Search([FromQuery] LineFilter filter)
        {
            return Ok(await lineService.QueryWithPage(filter));
        }
    }
}
