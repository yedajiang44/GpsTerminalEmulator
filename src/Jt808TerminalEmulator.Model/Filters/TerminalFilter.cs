using System.Linq.Expressions;
using Jt808TerminalEmulator.Model.Dtos;
using Jt808TerminalEmulator.Model.Enum;

namespace Jt808TerminalEmulator.Model.Filters;

public class TerminalFilter : BaseFilter<TerminalDto>
{
    public string Sim { get; set; }
    public string LicensePlate { get; set; }
    public OnlineStatus OnlineState { get; set; }

    public override List<(bool ifExpression, Expression<Func<TerminalDto, bool>> whereExpression)> WhereLambda() => new()
        {
            (!string.IsNullOrWhiteSpace(Sim),x=>x.Sim.Contains(Sim)),
            (!string.IsNullOrWhiteSpace(LicensePlate),x=>x.LicensePlate.Contains(LicensePlate))
        };
}