using Jt808TerminalEmulator.Repository.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace Jt808TerminalEmulator.Service;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(AutoMapperProfile));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        foreach (var x in typeof(UnitOfWork).Assembly.GetTypes().Where(x => !x.IsInterface && x.Namespace.StartsWith("Jt808TerminalEmulator.Repository.Repositorys")).Select(x => new { Implementation = x, Services = x.GetInterfaces() }))
        {
            foreach (var service in x.Services)
            {
                services.AddScoped(service, x.Implementation);
            }
        }
        //通过反射，批量取出需要注入的接口和实现类
        foreach (var x in typeof(DependencyInjectionExtensions).Assembly.GetTypes().Where(x => x.Namespace?.StartsWith("Jt808TerminalEmulator.Servic") == true && x.GetInterfaces().Any(x => x.Name.EndsWith("Service"))).Select(x => new { Implementation = x, Services = x.GetInterfaces() }))
        {
            foreach (var service in x.Services)
            {
                services.AddScoped(service, x.Implementation);
            }
        }
        return services;
    }
}