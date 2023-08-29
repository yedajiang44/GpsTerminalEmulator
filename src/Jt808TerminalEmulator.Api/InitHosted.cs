using Jt808TerminalEmulator.Core.Netty;
using Jt808TerminalEmulator.Interface;
using Jt808TerminalEmulator.Model.Filters;

namespace Jt808TerminalEmulator.Api;

public class InitHosted : IHostedService
{
    readonly ILogger logger;
    readonly LineManager lineManager;
    readonly ILineService lineService;
    readonly IDatabaseService databaseService;
    public InitHosted(ILogger<InitHosted> logger, IServiceScopeFactory serviceScopeFactory)
    {
        this.logger = logger;
        var services = serviceScopeFactory.CreateScope().ServiceProvider;
        lineManager = services.GetRequiredService<LineManager>();
        lineService = services.GetRequiredService<ILineService>();
        databaseService = services.GetRequiredService<IDatabaseService>();
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Init Database...");
        await databaseService.InitAsync();
        logger.LogInformation("Init LineManager...");
        lineManager.ResetLine(await lineService.Query<LineFilter>());
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}