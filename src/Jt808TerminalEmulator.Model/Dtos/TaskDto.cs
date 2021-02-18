using System.Collections.Generic;
using Jt808TerminalEmulator.Model.Enum;

namespace Jt808TerminalEmulator.Model.Dtos
{
    /// <summary>
    /// 任务
    /// </summary>
    public class TaskDto : BaseDto
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 定位上报间隔，单位：秒
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// 速度，单位：千米/小时
        /// </summary>
        public double Speed { get; set; }

        /// <summary>
        /// 终端卡号
        /// </summary>
        public string SimNumber { get; set; }

        /// <summary>
        /// 服务器Ip
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 服务器端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public TaskStatus Status { get; set; }

        /// <summary>
        /// 线路主键
        /// </summary>
        public string LineId { get; set; }
        /// <summary>
        /// 线路名称
        /// </summary>
        public string LineName { get; set; }

        /// <summary>
        /// 终端
        /// </summary>
        public IList<TerminalDto> Terminals { get; set; }
    }
}