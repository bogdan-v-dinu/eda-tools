using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CloudEventsDemo.ConsoleA
{
    public interface TestEvent
    {
        int Id { get; set; }
    }

    public class TestEventConsumer : IConsumer<TestEvent>
    {
        private readonly ILogger<TestEventConsumer> _logger;

        public TestEventConsumer()
        {
            _logger = HostHelpers.serviceProvider.GetService<ILogger<TestEventConsumer>>();
        }

        public async Task Consume(ConsumeContext<TestEvent> context)
        {
            _logger.LogInformation($"Consume: received TestEvent with Id {context.Message.Id}");
            await Task.CompletedTask;
        }
    }
}
