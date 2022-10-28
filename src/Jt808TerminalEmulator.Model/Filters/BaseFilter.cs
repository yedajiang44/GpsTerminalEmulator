using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Jt808TerminalEmulator.Model.Filters
{
    public abstract class BaseFilter<TDto>
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        public int Index { get; set; } = 1;

        /// <summary>
        /// 每页长度
        /// </summary>
        public int Size { get; set; } = 20;

        /// <summary>
        /// 排序表达式
        /// </summary>
        public string Sort { get; set; }

        public abstract List<(bool ifExpression, Expression<Func<TDto, bool>> whereExpression)> WhereLambda();
    }
}
