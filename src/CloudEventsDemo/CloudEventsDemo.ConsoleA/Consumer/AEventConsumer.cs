using CloudEventsDemo.ProducerContracts;
using CloudEventsDemo.Serialization;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace CloudEventsDemo.ConsoleA
{
    /// <summary>
    /// Sample consumer for events of type AEvent
    /// </summary>
    public class AEventConsumer : EventConsumer<AEvent>
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        public AEventConsumer()
        {
        }

        /// <summary>
        /// Ctor with CloudEvent(s) reader and logger
        /// </summary>
        /// <param name="ceReader"></param>
        /// <param name="logger"></param>
        public AEventConsumer(ICloudEventReader ceReader, ILogger<AEventConsumer> logger) : base(ceReader, logger)
        {
        }

        #region IConsumer<AEvent> implementation

        /// <summary>
        /// Consume <see cref="ProducerContracts.AEvent">AEvent(s)</see> 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task Consume(ConsumeContext<AEvent> context)
        {
            await ConsumeCloudEvent(context, payload =>
            {
                switch (payload)
                {
                    case ConsumerPayload consumerPayload:
                        _logger.LogInformation($"Consume: Payload for MessageId '{context.MessageId}' has mapped type = '{consumerPayload.GetType().Name}', content = '{JsonConvert.SerializeObject(consumerPayload)}'");
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
