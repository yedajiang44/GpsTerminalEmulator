using AutoMapper;
using Jt808TerminalEmulator.Data.Entity;
using Jt808TerminalEmulator.Interface;
using Jt808TerminalEmulator.Model.Dtos;
using Jt808TerminalEmulator.Repository.Repositorys;
using Jt808TerminalEmulator.Repository.UnitOfWork;

namespace Jt808TerminalEmulator.Service;

internal class TerminalService : BaseService<TerminalDto, TerminalEntity>, ITerminalService
{
    public TerminalService(IMapper mapper, IUnitOfWork unitOfWork, ITerminalRepository currentRepository) : base(mapper, unitOfWork, currentRepository)
    {
    }

    public async Task<int> AddRandom(int count)
    {
        await (currentRepository as ITerminalRepository)!.AddRandom(count);
        return await unitOfWork.SaveChangesAsync();
    }
}