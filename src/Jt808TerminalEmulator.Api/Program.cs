using Figgle;
using Jt808TerminalEmulator;
using Jt808TerminalEmulator.Api;
using Jt808TerminalEmulator.Api.Configurations;
using Jt808TerminalEmulator.Core;
using Jt808TerminalEmulator.Service;
using Microsoft.EntityFrameworkCore;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services
.AddHostedService<InitHosted>()
.AddJsonWebToken(builder.Configuration)
.UseJt808TerminalEmulator(builder.Configuration.GetSection("gateway"))
.UseServices()
.AddSwagger()
.AddDbContextPool<EmulatorDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), builder => builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)))
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

builder.Logging.ClearProviders();
builder.Host.UseNLog();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwaggerAndUI();
}
var services = app.Services;

app.UseRouting();
app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

services.GetService<ILogger<Program>>().LogInformation("{newline}{message}", Environment.NewLine, FiggleFonts.Standard.Render("H e l l o , J t 8 0 8 !"));
await app.RunAsync();