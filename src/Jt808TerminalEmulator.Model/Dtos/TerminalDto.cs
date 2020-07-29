using System;
using System.Collections.Generic;
using System.Text;

namespace Jt808TerminalEmulator.Model.Dtos
{
    public class TerminalDto : BaseDto
    {
        /// <summary>
        /// 终端手机号
        /// </summary>
        public string Sim { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string LicensePlate { get; set; }
    }
}
