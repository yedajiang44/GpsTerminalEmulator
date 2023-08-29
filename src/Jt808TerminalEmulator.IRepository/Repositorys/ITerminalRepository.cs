using Bogus;
using Jt808TerminalEmulator.Data.Entity;
using Jt808TerminalEmulator.Repository.Base;

namespace Jt808TerminalEmulator.Repository.Repositorys;

public interface ITerminalRepository : IBaseRepository<TerminalEntity>
{
    Task AddRandom(int count);
}
public class TerminalRepository : BaseRepository<TerminalEntity>, ITerminalRepository
{
    public TerminalRepository(EmulatorDbContext dbContext) : base(dbContext)
    {
    }

    public Task AddRandom(int count)
    {
        var entitys = new Faker<TerminalEntity>("zh_CN")
            .StrictMode(true)
            .RuleFor(x => x.Id, _ => Guid.NewGuid().ToString("N"))
            .RuleFor(x => x.IsDeleted, _ => false)
            .RuleFor(x => x.CreateUserId, _ => null)
            .RuleFor(x => x.CreateDateTime, x => DateTime.SpecifyKind(x.Date.Past(), DateTimeKind.Utc))
            .RuleFor(x => x.LicensePlate, x => x.Random.Replace("??#####"))
            .RuleFor(x => x.Sim, x => x.Phone.PhoneNumber("###########"))
            .Ignore(x => x.Tasks)
            .Generate(count);
        return dbContext.AddRangeAsync(entitys);
    }
}