using System;
using System.Collections.Generic;
using System.Text;

namespace Jt808TerminalEmulator.Model.Dtos
{
    /// <summary>
    /// 定位点
    /// </summary>
    public class LocationDto : BaseDto
    {
        /// <summary>
        /// 排序
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double Logintude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public double Latitude { get; set; }
    }
}
