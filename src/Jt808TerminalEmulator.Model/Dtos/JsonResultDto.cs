using System;
using System.Collections.Generic;
using System.Text;

namespace Jt808TerminalEmulator.Model.Dtos
{
    public class JsonResultDto
    {
        public bool Flag { get; set; }
        public dynamic Data { get; set; }
        public string Message { get; set; }
    }
}
