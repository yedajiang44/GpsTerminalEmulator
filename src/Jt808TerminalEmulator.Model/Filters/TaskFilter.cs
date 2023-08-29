using System.Linq.Expressions;
using Jt808TerminalEmulator.Model.Dtos;

namespace Jt808TerminalEmulator.Model.Filters;

public class TaskFilter : BaseFilter<TaskDto>
{
    /// <summary>
    /// 别名
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 线路名称
    /// </summary>
    public string LineName { get; set; }
    /// <summary>
    /// 定位间隔
    /// </summary>
    public int Interval { get; set; }
    /// <summary>
    /// 速度
    /// </summary>
    public int Speed { get; set; }
    public override List<(bool ifExpression, Expression<Func<TaskDto, bool>> whereExpression)> WhereLambda() => new()
        {
            (!string.IsNullOrWhiteSpace(Name),x=>x.Name.Contains(Name)),
            (!string.IsNullOrWhiteSpace(LineName),x=>x.LineName.Contains(LineName)),
            (Interval>0,x=>x.Interval>0),
            (Speed>0,x=>x.Speed>0)
        };
}