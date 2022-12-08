using Figgle;
using Jt808TerminalEmulator;
using Jt808TerminalEmulator.Api;
using Jt808TerminalEmulator.Api.Configurations;
using Jt808TerminalEmulator.Core;
using Jt808TerminalEmulator.Service;
using Microsoft.EntityFrameworkCore;
using NLog.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureServices((hostContext, services) =>
{
    var Configuration = hostContext.Configuration;
    services.AddHostedService<InitHosted>()
    .AddJsonWebToken(Configuration)
    .UseJt808TerminalEmulator(Configuration.GetSection("gateway"))
    .UseServices()
    .AddSwagger()
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
});
var app = builder.Build();

{
    app.UseDeveloperExceptionPage();
    app.UseSwaggerAndUI();
}
var services = app.Services.CreateScope().ServiceProvider;

app.UseRouting();
app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

services.GetService<ILogger<Program>>().LogInformation("{newline}{message}", Environment.NewLine, FiggleFonts.Standard.Render("H e l l o , J t 8 0 8 !"));
await app.RunAsync();