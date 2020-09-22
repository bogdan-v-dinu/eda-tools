using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;

namespace CloudEventsDemo.ConsoleA
{
    /// <summary>
    /// A consumer definition can be used for explicit consumer registration and configuration
    /// </summary>
    public class AEventConsumerDefinition : ConsumerDefinition<AEventConsumer>
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        public AEventConsumerDefinition()
        {
            ConcurrentMessageLimit = 1; // limit no of AEvent messages consumed concurrently for debugging & logging purposes 
        }

        /// <summary>
        /// Configure consumer
        /// </summary>
        /// <param name="endpointConfigurator"></param>
        /// <param name="consumerConfigurator"></param>
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
           IConsumerConfigurator<AEventConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseInMemoryOutbox();
        }
    }
}
