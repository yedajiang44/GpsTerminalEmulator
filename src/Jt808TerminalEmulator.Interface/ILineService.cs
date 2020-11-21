using Jt808TerminalEmulator.Model.Dtos;
using Jt808TerminalEmulator.Model.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jt808TerminalEmulator.Interface
{
    public interface ILineService
    {
        Task<int> Add(LineDto dto);
        Task<int> Delete(string[] ids);
        Task<int> Update(LineDto dto);
        Task<LineDto> Fine(string id);
        Task<PageResultDto<LineDto>> Search(LineFilter filter);
    }
}
