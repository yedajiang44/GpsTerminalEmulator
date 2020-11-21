using System;
using System.Collections.Generic;
using System.Text;

namespace Jt808TerminalEmulator.Model.Filters
{
    public class BaseFilter
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 每页长度
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 排序表达式
        /// </summary>
        public string Sort { get; set; }
    }
}
