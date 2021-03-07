using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jt808TerminalEmulator.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly EmulatorDbContext dbContext;
        public UnitOfWork(EmulatorDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public int SaveChanges() => dbContext.SaveChanges();

        public Task<int> SaveChangesAsync() => dbContext.SaveChangesAsync();
    }
}
