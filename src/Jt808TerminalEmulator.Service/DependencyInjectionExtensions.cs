using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jt808TerminalEmulator.Service
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection UseServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperProfile));
            //通过反射，批量取出需要注入的接口和实现类
            var registrations =
                from type in typeof(DependencyInjectionExtensions).Assembly.GetTypes()
                where type.Namespace != null && (type.Namespace.StartsWith("Jt808TerminalEmulator.Service") &&
                                               type.GetInterfaces().Any(x => x.Name.EndsWith("Service")) &&
                                               type.GetInterfaces().Any())
                select new { Service = type.GetInterfaces().First(), Implementation = type };

            foreach (var t in registrations)
            {
                services.AddScoped(t.Service, t.Implementation);
            }
            return services;
        }
    }
}
