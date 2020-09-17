using Microsoft.Extensions.Hosting;
using System;

namespace CloudEventsDemo.ConsoleA
{
    public static class HostHelpers
    {
        public static IServiceProvider serviceProvider;

        public static IHost GetServiceProvider(this IHost host)
        {
            serviceProvider = host.Services;
            return host;
        }
    }
}
