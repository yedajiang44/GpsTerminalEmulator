using Jt808TerminalEmulator.Interface;
using Microsoft.EntityFrameworkCore;

namespace Jt808TerminalEmulator.Service;

internal class DatabaseService : IDatabaseService
{
    readonly EmulatorDbContext dbContext;

    public DatabaseService(EmulatorDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task InitAsync()
    {
        await dbContext.Database.MigrateAsync();
    }
}