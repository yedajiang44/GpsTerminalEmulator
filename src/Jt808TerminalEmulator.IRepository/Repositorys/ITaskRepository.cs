using Jt808TerminalEmulator.Data.Entity;
using Jt808TerminalEmulator.Repository.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Jt808TerminalEmulator.Repository.Repositorys;

public interface ITaskRepository : IBaseRepository<TaskEntity> { }
public class TaskRepository : BaseRepository<TaskEntity>, ITaskRepository
{
    public TaskRepository(EmulatorDbContext dbContext) : base(dbContext)
    {
    }
    public override IQueryable<TaskEntity> BaseQuery() => dbContext.Set<TaskEntity>().Include(x => x.Line).Include(x => x.Terminals);

    public override async ValueTask<EntityEntry<TaskEntity>> Add(TaskEntity entity)
    {
        var terminals = entity.Terminals.Select(x => x.Id).ToList();
        entity.Terminals.Clear();
        var result = await dbContext.Set<TaskEntity>().AddAsync(entity);
        await dbContext.SaveChangesAsync();
        entity = await dbContext.Set<TaskEntity>().FindAsync(entity.Id);
        dbContext.Set<TerminalEntity>().Where(x => terminals.Contains(x.Id)).ToList().ForEach(x => entity.Terminals.Add(x));
        return result;
    }
}