namespace Jt808TerminalEmulator.Data.Entity;

public class TerminalEntity : BaseEntity
{
    public string Sim { get; set; }
    public string LicensePlate { get; set; }
    public IList<TaskEntity> Tasks { get; set; }
}