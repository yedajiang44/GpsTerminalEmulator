using Jt808TerminalEmulator.Model.Dtos;
using Jt808TerminalEmulator.Model.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jt808TerminalEmulator.Interface
{
    public interface ITerminalService
    {
        /// <summary>
        /// 随机生成模型
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<int> AddRandom(int count);
        Task<bool> Add(TerminalDto dto);
        Task<bool> Delete(string[] ids);
        Task<bool> DeleteAll();
        Task Update(TerminalDto dto);
        Task<TerminalDto> Find(string id);
        Task<IList<TerminalDto>> FindAll();
        Task<PageResultDto<TerminalDto>> Search(TerminalFilter filter);
    }
}
