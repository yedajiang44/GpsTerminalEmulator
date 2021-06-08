using Figgle;
using Jt808TerminalEmulator.Api.Configurations;
using Jt808TerminalEmulator.Core;
using Jt808TerminalEmulator.Core.Netty;
using Jt808TerminalEmulator.Interface;
using Jt808TerminalEmulator.Model.Filters;
using Jt808TerminalEmulator.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

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
                .AddLogging(logger => logger.ClearProviders().AddNLog(new NLogLoggingConfiguration(Configuration.GetSection("NLog"))))
                .AddDbContextPool<EmulatorDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"), builder => builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)))
                // .AddAuthorization(options =>
                // {
                //     options.AddPolicy("Client", policy => policy.RequireRole("Client").Build());
                //     options.AddPolicy("Admin", policy => policy.RequireRole("Admin").Build());
                //     //这个是并的关系
                //     options.AddPolicy("AdminAndClient", policy => policy.RequireRole("Admin,Client").Build());
                //     //这个是或的关系
                //     options.AddPolicy("SystemOrAdmin", policy => policy.RequireRole("Admin", "System").Build());
                // })
                .AddControllers()
                .AddJsonDateTimeConverters();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            var services = app.ApplicationServices.CreateScope().ServiceProvider;
            services.GetService<IDatabaseService>().InitAsync().Wait();
            var lines = services.GetRequiredService<ILineService>().Query<LineFilter>().Result;
            services.GetService<LineManager>().ResetLine(lines);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("cors");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            services.GetService<ILogger<Startup>>().LogInformation($"\r\n{FiggleFonts.Standard.Render("H e l l o , J t 8 0 8 !")}");
        }
    }
}
