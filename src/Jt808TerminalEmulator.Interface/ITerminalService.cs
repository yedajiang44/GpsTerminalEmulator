using Jt808TerminalEmulator.Model.Dtos;
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
        Task Add(TerminalDto dto);
        Task<bool> Delete(string[] ids);
        Task Update(TerminalDto dto);
        Task<TerminalDto> Find(string id);
        Task<IList<TerminalDto>> FindAll();
    }
}
