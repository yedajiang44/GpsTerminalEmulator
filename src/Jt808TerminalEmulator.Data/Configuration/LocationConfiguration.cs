using Jt808TerminalEmulator.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jt808TerminalEmulator.Data.Configuration
{
    public class LocationConfiguration : BaseConfiguration<LocationEntity>
    {
        public override void Configure(EntityTypeBuilder<LocationEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("Location");
            builder.HasOne(x => x.Line);
        }
    }
}
