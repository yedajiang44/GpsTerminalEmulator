using Jt808TerminalEmulator.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jt808TerminalEmulator.Data.Configuration
{
    public class TaskConfiguration : BaseConfiguration<TaskEntity>
    {
        public override void Configure(EntityTypeBuilder<TaskEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("Task");
        }
    }
}
