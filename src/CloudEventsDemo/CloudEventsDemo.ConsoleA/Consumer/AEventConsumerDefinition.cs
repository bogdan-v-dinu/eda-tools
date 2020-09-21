using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;

namespace CloudEventsDemo.ConsoleA
{
    public class AEventConsumerDefinition : ConsumerDefinition<AEventConsumer>
    {
        public AEventConsumerDefinition()
        {
            ConcurrentMessageLimit = 10; // no of AEvent messages consumed concurrently
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
           IConsumerConfigurator<AEventConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseInMemoryOutbox();
        }
    }
}
