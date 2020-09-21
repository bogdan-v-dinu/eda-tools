using CloudEventsDemo.Contracts;
using CloudEventsDemo.Serialization;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace CloudEventsDemo.ConsoleA.Consumer
{
    public class CEventConsumer : IConsumer<IGenericEvent<CEvent>>
    {
        ICloudEventReader _ceReader;

        private readonly ILogger<CEventConsumer> _logger;

        public CEventConsumer()
        {
            this._ceReader = HostHelpers.serviceProvider.GetService<ICloudEventReader>();
            this._logger = HostHelpers.serviceProvider.GetService<ILogger<CEventConsumer>>();
        }

        public CEventConsumer(ICloudEventReader ceReader, ILogger<CEventConsumer> logger)
        {
            this._ceReader = ceReader;
            this._logger = logger;
        }


        #region IConsumer<IGenericPublisherEvent<CEvent>> implementation

        public async Task Consume(ConsumeContext<IGenericEvent<CEvent>> context)
        {
            try
            {
                _logger.LogInformation($"Consume: received IGenericEvent<CEvent> with MessageId = '{context.MessageId}'");

                object payload = _ceReader.GetPayload(context.Message.EventData);

                switch (payload)
                {
                    case ConsumerPayload consumerPayload:
                        _logger.LogInformation($"Consume: Payload for MessageId '{context.MessageId}' has mapped type = '{consumerPayload.GetType().Name}', content = '{JsonConvert.SerializeObject(consumerPayload)}'");
                        break;
                    default:
                        _logger.LogWarning($"'{this.GetType().Name}' cannot consume payloads of type {payload.GetType().Name}");
                        break;
                }
            }
            catch (Exception ex)
            {
                var errDetails = ex.InnerException != null ?
                    ex.InnerException.GetType().Name + " - " + ex.InnerException.Message :
                    ex.StackTrace ?? "NA";

                _logger.LogError($"Consume: failed with exception '{ex.Message}', of type '{ex.GetType().Name}', details '{errDetails}'");
            }

            await Task.CompletedTask;
        }

        #endregion
    }
}
