using Jt808TerminalEmulator.Data.Entity;
using Jt808TerminalEmulator.Repository.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jt808TerminalEmulator.Repository.Repositorys
{
    public interface ILineRepository : IBaseRepository<LineEntity> { }
    public class LineRepository : BaseRepository<LineEntity>, ILineRepository
    {
        public LineRepository(EmulatorDbContext dbContext) : base(dbContext)
        {
        }
    }
}
