using System;
using System.Collections.Generic;
using System.Text;

namespace Jt808TerminalEmulator.Data.Entity
{
    /// <summary>
    /// 定位点实体
    /// </summary>
    public class LocationEntity : BaseEntity
    {
        /// <summary>
        /// 经度
        /// </summary>
        public double Logintude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// 线路主键
        /// </summary>
        public string LineId { get; set; }

        /// <summary>
        /// 所属线路
        /// </summary>
        public LineEntity Line { get; set; }
    }
}
