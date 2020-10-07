using System.Collections.Generic;
using System.Threading.Tasks;
using Jt808TerminalEmulator.Interface;
using Jt808TerminalEmulator.Model.Dtos;
using Jt808TerminalEmulator.Model.Filters;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> Find(string id)
        {
            return Ok(new JsonResultDto<TerminalDto> { Data = await terminalService.Find(id) });
        }

        [HttpGet]
        public async Task<IActionResult> FindAll()
        {
            return Ok(new JsonResultDto<IList<TerminalDto>> { Data = await terminalService.FindAll() });
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
            return Ok(new JsonResultDto<bool>
            {
                Data = result,
                Message = result ? null : "操作失败"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await terminalService.Delete(new string[] { id });
            return Ok(new JsonResultDto<bool>
            {
                Data = result,
                Message = result ? null : "操作失败"
            });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] string[] ids)
        {
            var result = await terminalService.Delete(ids);
            return Ok(new JsonResultDto<bool>
            {
                Data = result,
                Message = result ? null : "操作失败"
            });
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteAll()
        {
            var result = await terminalService.DeleteAll();
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
            return Ok(await terminalService.Search(filter));
        }
    }
}
