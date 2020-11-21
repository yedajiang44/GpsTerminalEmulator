using System;
using System.Collections.Generic;
using System.Text;

namespace Jt808TerminalEmulator.Model.Dtos
{
    /// <summary>
    /// 线路
    /// </summary>
    public class LineDto : BaseDto
    {
        /// <summary>
        /// 别名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 距离
        /// </summary>
        public decimal Distance { get; set; }
        /// <summary>
        /// 基准点数量
        /// </summary>
        public int LocationCount { get; set; }
        /// <summary>
        /// 定位上报间隔，单位：秒
        /// </summary>
        public int Interval { get; set; }
        /// <summary>
        /// 速度，单位：千米/小时
        /// </summary>
        public double Speed { get; set; }
        /// <summary>
        /// 基准点
        /// </summary>
        public List<LocationDto> Locations { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }
    }
}
