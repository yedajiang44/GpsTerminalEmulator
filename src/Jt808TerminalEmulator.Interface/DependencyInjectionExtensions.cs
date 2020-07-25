using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jt808TerminalEmulator.Interface
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection UseService(this IServiceCollection services, IConfiguration configuration)
        {

            return services;
        }
    }
}
