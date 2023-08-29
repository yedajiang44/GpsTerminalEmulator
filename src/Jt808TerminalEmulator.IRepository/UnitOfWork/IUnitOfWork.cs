namespace Jt808TerminalEmulator.Repository.UnitOfWork;

public interface IUnitOfWork : IUnitOfWork<EmulatorDbContext>
{
}
public interface IUnitOfWork<TDbContext>
{
    TDbContext DbContext { get; }
    Task<int> SaveChangesAsync();
    int SaveChanges();
}