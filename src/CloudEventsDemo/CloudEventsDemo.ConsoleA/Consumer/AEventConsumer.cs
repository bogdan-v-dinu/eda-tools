using CloudEventsDemo.Contracts;
using CloudEventsDemo.Serialization;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace CloudEventsDemo.ConsoleA
{
    public class AEventConsumer : IConsumer<AEvent>
    {
        ICloudEventReader _ceReader;

        private readonly ILogger<AEventConsumer> _logger;

        public AEventConsumer()
        {
            this._ceReader = HostHelpers.serviceProvider.GetService<ICloudEventReader>();
            this._logger = HostHelpers.serviceProvider.GetService<ILogger<AEventConsumer>>();
        }

        public AEventConsumer(ICloudEventReader ceReader, ILogger<AEventConsumer> logger)
        {
            this._ceReader = ceReader;
            this._logger = logger;
        }

        #region IConsumer<AEvent> implementation

        public async Task Consume(ConsumeContext<AEvent> context)
        {
            try
            {
                _logger.LogInformation($"Consume: received AEvent with MessageId = '{context.MessageId}'");

                object payload = _ceReader.GetPayload(context.Message.EventData);
                
                switch( payload )
                {
                    case ConsumerPayload consumerPayload:
                        _logger.LogInformation($"Consume: MessageId '{context.MessageId}' has payload type = '{consumerPayload.GetType().Name}', content = '{JsonConvert.SerializeObject(consumerPayload)}'");
                        break;
                    default:
                        _logger.LogWarning($"'{typeof(AEventConsumer)}' cannot consume payloads of type {payload.GetType().Name}");
                        break;
                }
            }
            catch(Exception ex)
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
