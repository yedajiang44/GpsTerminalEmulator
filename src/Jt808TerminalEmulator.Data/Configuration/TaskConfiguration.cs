using Jt808TerminalEmulator.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jt808TerminalEmulator.Data.Configuration
{
    public class TaskConfiguration : BaseConfiguration<TaskEntity>
    {
        public override void Configure(EntityTypeBuilder<TaskEntity> builder)
        {
            base.Configure(builder);
            builder.ToTable("Task");
            builder.HasOne(x => x.Line).WithOne(x => x.Task).HasForeignKey<TaskEntity>(x => x.LineId).IsRequired();
            builder.HasMany(x => x.Terminals).WithMany(x => x.Tasks).UsingEntity(x=>x.ToTable("TaskTerminals"));
        }
    }
}
