using Jt808TerminalEmulator.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jt808TerminalEmulator.Data.Configuration
{
    public class TerminalConfiguration : BaseConfiguration<TerminalEntity>
    {
        public override void Configure(EntityTypeBuilder<TerminalEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("Terminal");
        }
    }
}
