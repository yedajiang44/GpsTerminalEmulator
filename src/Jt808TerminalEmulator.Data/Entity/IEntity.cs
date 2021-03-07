namespace Jt808TerminalEmulator.Data.Entity
{
    public interface IEntity<TPrimaryKey>
    {
         TPrimaryKey Id{ get; set;}
    }
}