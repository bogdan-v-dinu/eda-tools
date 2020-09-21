using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;

namespace CloudEventsDemo.ConsoleA
{
    public class AEventConsumerDefinition : ConsumerDefinition<AEventConsumer>
    {
        public AEventConsumerDefinition()
        {
            ConcurrentMessageLimit = 1; // limit no of AEvent messages consumed concurrently for debugging & logging purposes 
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
           IConsumerConfigurator<AEventConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseInMemoryOutbox();
        }
    }
}
