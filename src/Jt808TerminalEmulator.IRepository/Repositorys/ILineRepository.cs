using Jt808TerminalEmulator.Data.Entity;
using Jt808TerminalEmulator.Repository.Base;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Jt808TerminalEmulator.Repository.Repositorys
{
    public interface ILineRepository : IBaseRepository<LineEntity> { }
    public class LineRepository : BaseRepository<LineEntity>, ILineRepository
    {
        public LineRepository(EmulatorDbContext dbContext) : base(dbContext)
        {
        }
        public override IQueryable<LineEntity> BaseQuery() => dbContext.Set<LineEntity>().Include(x => x.Locations);
    }
}
