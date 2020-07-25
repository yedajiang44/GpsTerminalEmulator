using Jt808TerminalEmulator.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jt808TerminalEmulator.Service
{
    public class TerminalService : ITerminalService
    {
        readonly EmulatorDbContext emulatorDbContext;
        public TerminalService(EmulatorDbContext emulatorDbContext)
        {
            this.emulatorDbContext = emulatorDbContext;
        }
    }
}
