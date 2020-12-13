using System;
using System.Collections.Generic;
using System.Text;

namespace Jt808TerminalEmulator.Data.Entity
{
    public class TaskEntity : BaseEntity
    {
        /// <summary>
        /// 别名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
    }
}
