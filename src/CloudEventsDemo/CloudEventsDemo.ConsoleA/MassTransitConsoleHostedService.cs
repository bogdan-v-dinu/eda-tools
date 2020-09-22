using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CloudEventsDemo.ConsoleA
{
    /// <summary>
    /// Bootstraps the MassTransit bus control
    /// </summary>
    public class MassTransitConsoleHostedService : IHostedService
    {
        readonly IBusControl _bus;
        readonly ILogger _logger;

        /// <summary>
        /// Ctor with bus control and logger
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="loggerFactory"></param>
        public MassTransitConsoleHostedService(IBusControl bus, ILoggerFactory loggerFactory)
        {
            _bus = bus;
            _logger = loggerFactory.CreateLogger<MassTransitConsoleHostedService>();
        }

        /// <summary>
        /// Start MassTransit bus
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting bus for ConsoleA");
            await _bus.StartAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Stop MassTransit bus
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping bus ConsoleA");
            return _bus.StopAsync(cancellationToken);
        }
    }

}
