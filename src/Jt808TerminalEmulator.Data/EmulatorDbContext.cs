using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Jt808TerminalEmulator
{
    public class EmulatorDbContext : DbContext
    {
        public EmulatorDbContext(DbContextOptions<EmulatorDbContext> options) : base(options)
        {

        }
        /// <summary>
        /// OnModelCreating
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //添加FluentAPI配置
            GetType().Assembly.GetTypes().Where(q => q.GetInterface(typeof(IEntityTypeConfiguration<>).FullName) != null && !q.FullName.Contains("BaseConfiguration")).ToList()
                .ForEach(type =>
                {
                    dynamic configurationInstance = Activator.CreateInstance(type);
                    modelBuilder.ApplyConfiguration(configurationInstance);
                });
        }
    }
}
