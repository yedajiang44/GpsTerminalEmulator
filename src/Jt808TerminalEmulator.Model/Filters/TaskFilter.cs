using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Jt808TerminalEmulator.Model.Dtos;

namespace Jt808TerminalEmulator.Model.Filters
{
    public class TaskFilter : BaseFilter<TaskDto>
    {
        public string Name { get; set; }
        public override List<(bool ifExpression, Expression<Func<TaskDto, bool>> whereExpression)> WhereLambda() => new List<(bool ifExpression, Expression<Func<TaskDto, bool>> whereExpression)>
        {
            (!string.IsNullOrWhiteSpace(Name),x=>x.Name.Contains(Name))
        };
    }
}