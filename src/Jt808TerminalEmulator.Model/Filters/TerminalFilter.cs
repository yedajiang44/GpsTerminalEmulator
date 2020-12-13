using Jt808TerminalEmulator.Model.Dtos;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Jt808TerminalEmulator.Model.Filters
{
    public class TerminalFilter : BaseFilter<TerminalDto>
    {
        public string Sim { get; set; }
        public string LicensePlate { get; set; }

        public override List<(bool ifExpression, Expression<Func<TerminalDto, bool>> whereExpression)> WhereLambda() => new List<(bool ifExpression, Expression<Func<TerminalDto, bool>> whereExpression)>
        {
            (!string.IsNullOrWhiteSpace(Sim),x=>x.Sim.Contains(Sim)),
            (!string.IsNullOrWhiteSpace(LicensePlate),x=>x.LicensePlate.Contains(LicensePlate))
        };
    }
}
