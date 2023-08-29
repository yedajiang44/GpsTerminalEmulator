using AutoMapper;
using Jt808TerminalEmulator.Data.Entity;
using Jt808TerminalEmulator.Interface;
using Jt808TerminalEmulator.Model.Dtos;
using Jt808TerminalEmulator.Repository.Base;
using Jt808TerminalEmulator.Repository.UnitOfWork;

namespace Jt808TerminalEmulator.Service;

internal class TaskService : BaseService<TaskDto, TaskEntity>, ITaskService
{
    public TaskService(IMapper mapper, IUnitOfWork unitOfWork, IBaseRepository<TaskEntity> currentRepository) : base(mapper, unitOfWork, currentRepository)
    {
    }
}