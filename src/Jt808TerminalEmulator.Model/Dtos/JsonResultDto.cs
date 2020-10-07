using System;
using System.Collections.Generic;
using System.Text;

namespace Jt808TerminalEmulator.Model.Dtos
{
    public class JsonResultDto<T>
    {
        public bool Flag { get; set; } = true;
        public T Data { get; set; } = default;
        public string Message { get; set; }
    }
}
