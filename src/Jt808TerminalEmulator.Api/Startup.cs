using Jt808TerminalEmulator.Api.Configurations;
using Jt808TerminalEmulator.Core;
using Jt808TerminalEmulator.Interface;
using Jt808TerminalEmulator.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Jt808TerminalEmulator.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddJsonWebToken(Configuration)
                .UseJt808TerminalEmulator()
                .UseServices()
                // .AddDbContext<EmulatorDbContext>(options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new System.Version())))
                .AddDbContext<EmulatorDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")))
#if debug
                // .AddCors(option => option.AddPolicy("cors", policy => policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(new[] { "http://localhost:4200" })))
#endif
                .AddControllers().AddJsonDateTimeConverters();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.ApplicationServices.CreateScope().ServiceProvider.GetService<IDatabaseService>().InitAsync().Wait();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("cors");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
