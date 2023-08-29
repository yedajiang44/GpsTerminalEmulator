using Jt808TerminalEmulator.Model.Dtos;

namespace Jt808TerminalEmulator.Interface;

public interface ITerminalService : IBaseService<TerminalDto>
{
    /// <summary>
    /// 随机生成模型
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    Task<int> AddRandom(int count);
}