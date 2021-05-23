using System;
using System.Collections.Generic;
using System.Text;

namespace Jt808TerminalEmulator.Model.Dtos
{
    public class PageResultDto<T>
    {
        public long Total { get; set; }

        public IEnumerable<T> List { get; set; }
    }
}
