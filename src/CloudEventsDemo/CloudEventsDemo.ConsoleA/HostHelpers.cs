using Microsoft.Extensions.Hosting;
using System;

namespace CloudEventsDemo.ConsoleA
{
    /// <summary>
    /// Generic host helpers
    /// </summary>
    public static class HostHelpers
    {
        /// <summary>
        /// Service provider
        /// </summary>
        public static IServiceProvider serviceProvider;

        /// <summary>
        /// Get the service provider from the <see cref="IHost"/>
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static IHost GetServiceProvider(this IHost host)
        {
            serviceProvider = host.Services;
            return host;
        }
    }
}
