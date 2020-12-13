using Jt808TerminalEmulator.Model.Dtos;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Jt808TerminalEmulator.Model.Filters
{
    /// <summary>
    /// 线路过滤器
    /// </summary>
    public class LineFilter : BaseFilter<LineDto>
    {
        /// <summary>
        /// 定位间隔
        /// </summary>
        public int Interval { get; set; }
        /// <summary>
        /// 速度
        /// </summary>
        public int Speed { get; set; }

        public override List<(bool ifExpression, Expression<Func<LineDto, bool>> whereExpression)> WhereLambda() => new List<(bool ifExpression, Expression<Func<LineDto, bool>> whereExpression)>
        {
            (Interval>0,x=>x.Interval>0),
            (Speed>0,x=>x.Speed>0)
        };
    }
}
