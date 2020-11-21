using Jt808TerminalEmulator.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jt808TerminalEmulator.Data.Configuration
{
    public class LineConfiguration : BaseConfiguration<LineEntity>
    {
        public override void Configure(EntityTypeBuilder<LineEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("Line");
            builder.HasMany(x => x.Locations).WithOne(x => x.Line).HasForeignKey(x => x.LineId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
