﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jt808TerminalEmulator.Repository.UnitOfWork
{
    public interface IUnitOfWork
    {
        EmulatorDbContext GetDbContext();

        Task<int> SaveChangesAsync();
    }
}
