using Microsoft.Extensions.DependencyInjection;

namespace Jt808TerminalEmulator.Interface;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseService(this IServiceCollection services)
    {
        return services;
    }
}