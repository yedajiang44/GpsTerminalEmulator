using System;
using System.Collections.Generic;
using System.Text;

namespace Jt808TerminalEmulator.Model.Filters
{
    /// <summary>
    /// 线路过滤器
    /// </summary>
    public class LineFilter : BaseFilter
    {
        /// <summary>
        /// 定位间隔
        /// </summary>
        public int Interval { get; set; }
        /// <summary>
        /// 速度
        /// </summary>
        public int Speed { get; set; }
    }
}
