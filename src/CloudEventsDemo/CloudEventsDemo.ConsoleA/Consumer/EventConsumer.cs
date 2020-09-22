using CloudEventsDemo.Contracts;
using CloudEventsDemo.MessageBrokerExtensions;
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
    /// Abstract base class for event consumers working with CloudEvents.
    /// Defined in the scope/domain of a consumer process
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class EventConsumer<T> : IConsumer<T> where T : class, IEvent
    {
        /// <summary>
        /// Reader for CloudEvent envelopes
        /// </summary>
        protected readonly ICloudEventReader _ceReader;

        /// <summary>
        /// Logger
        /// </summary>
        protected readonly ILogger<EventConsumer<T>> _logger;

        /// <summary>
        /// Default ctor
        /// </summary>
        public EventConsumer()
        {
            this._ceReader = HostHelpers.serviceProvider.GetService<ICloudEventReader>();
            this._logger = HostHelpers.serviceProvider.GetService<ILogger<EventConsumer<T>>>();
        }

        /// <summary>
        /// Ctor with CloudEvent(s) reader and logger
        /// </summary>
        /// <param name="ceReader"></param>
        /// <param name="logger"></param>
        public EventConsumer(ICloudEventReader ceReader, ILogger<EventConsumer<T>> logger)
        {
            this._ceReader = ceReader;
            this._logger = logger;
        }

        /// <summary>
        /// Perform a synchronous operation with the CloudEvent payload
        /// </summary>
        /// <param name="context"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        protected async Task ConsumeCloudEvent(ConsumeContext<T> context, Action<object> action)
        {
            try
            {
                _logger.LogInformation($"Consume: received '{typeof(T)}' with MessageId = '{context.MessageId}'");
                object payload = context.GetCloudEventsPayload(_ceReader);
                action(payload);
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

        /// <summary>
        /// Perform an asynchronous operation with the CloudEvent payload
        /// </summary>
        /// <param name="context"></param>
        /// <param name="asyncAction"></param>
        /// <returns></returns>
        protected async Task ConsumeCloudEvent(ConsumeContext<T> context, Func<object,Task> asyncAction)
        {
            try
            {
                _logger.LogInformation($"Consume: received '{typeof(T)}' with MessageId = '{context.MessageId}'");

                object payload = context.GetCloudEventsPayload(_ceReader);

                await asyncAction(payload);

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
            }
            catch (Exception ex)
            {
                var errDetails = ex.InnerException != null ?
                    ex.InnerException.GetType().Name + " - " + ex.InnerException.Message :
                    ex.StackTrace ?? "NA";

                _logger.LogError($"Consume: failed with exception '{ex.Message}', of type '{ex.GetType().Name}', details '{errDetails}'");
            }
        }

        #region IConsumer<T> implementation

        /// <summary>
        /// Abstract consumer for an event of type <see cref="IEvent"/>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract Task Consume(ConsumeContext<T> context);

        #endregion
    }
}
