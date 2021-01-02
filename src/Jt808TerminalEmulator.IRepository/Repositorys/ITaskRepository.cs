using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Jt808TerminalEmulator.Data.Entity;
using Jt808TerminalEmulator.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Jt808TerminalEmulator.Repository.Repositorys
{
    public interface ITaskRepository : IBaseRepository<TaskEntity> { }
    public class TaskRepository : BaseRepository<TaskEntity>, ITaskRepository
    {
        public TaskRepository(EmulatorDbContext dbContext) : base(dbContext)
        {
        }
        public override IQueryable<TaskEntity> BaseQuery() => dbContext.Set<TaskEntity>().Include(x => x.Line);
    }
}
