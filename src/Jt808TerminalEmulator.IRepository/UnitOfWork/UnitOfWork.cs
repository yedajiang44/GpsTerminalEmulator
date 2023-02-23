using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jt808TerminalEmulator.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public EmulatorDbContext DbContext { get; }

        public UnitOfWork(EmulatorDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public int SaveChanges() => DbContext.SaveChanges();

        public Task<int> SaveChangesAsync() => DbContext.SaveChangesAsync();
    }
}
