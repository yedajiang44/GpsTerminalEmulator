using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jt808TerminalEmulator.Repository.UnitOfWork
{
    public interface IUnitOfWork : IUnitOfWork<EmulatorDbContext>
    {
    }
    public interface IUnitOfWork<TDbContext>
    {
        TDbContext DbContext { get; }
        Task<int> SaveChangesAsync();
        int SaveChanges();
    }
}
