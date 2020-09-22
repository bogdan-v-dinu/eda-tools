using CloudEventsDemo.Contracts;
using CloudEventsDemo.ProducerContracts;
using CloudEventsDemo.Serialization;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace CloudEventsDemo.ConsoleA
{
    /// <summary>
    /// Sample consumer for events of type IGenericEvent&lt;CEvent&gt;
    /// </summary>
    public class CEventConsumer : EventConsumer<IGenericEvent<CEvent>>
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        public CEventConsumer() 
        { 
        }

        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="ceReader"></param>
        /// <param name="logger"></param>
        public CEventConsumer(ICloudEventReader ceReader, ILogger<CEventConsumer> logger) : base(ceReader, logger)
        {
        }

        #region IConsumer<IGenericPublisherEvent<CEvent>> implementation

        /// <summary>
        /// Consume <see cref="IGenericEvent{T}"/> where T is <see cref="ProducerContracts.CEvent"/>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task Consume(ConsumeContext<IGenericEvent<CEvent>> context)
        {
            await ConsumeCloudEvent(context, payload =>
            {
                switch (payload)
                {
                    case ConsumerPayload consumerPayload:
                        _logger.LogInformation(
                            $"Consume: Payload for MessageId '{context.MessageId}' has mapped type = '{consumerPayload.GetType().Name}', content = '{JsonConvert.SerializeObject(consumerPayload)}'");
                        break;
                    default:
                        _logger.LogWarning($"'{this.GetType().Name}' cannot consume payloads of type {payload.GetType().Name}");
                        break;
                }
            });
        }

        #endregion
    }
}
