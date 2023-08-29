namespace Jt808TerminalEmulator.Model.Dtos;

public interface IBaseDto<TPrimaryKey>
{
    TPrimaryKey Id { get; set; }
}