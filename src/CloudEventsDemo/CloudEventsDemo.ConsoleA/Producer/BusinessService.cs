using CloudEventsDemo.Contracts;
using CloudEventsDemo.MessageBrokerExtensions;
using CloudEventsDemo.ProducerContracts;
using CloudEventsDemo.Serialization;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CloudEventsDemo.ConsoleA.Publisher
{
    /// <summary>
    /// Interface for a sample service which publishes events
    /// </summary>
    public interface IBusinessService
    {
        /// <summary>
        /// Publish an <see cref="AEvent">AEvent</see> with <see cref="BasicPayload">BasicPayload</see>
        /// </summary>
        /// <param name="requestContextInfo"></param>
        /// <param name="cToken"></param>
        /// <returns></returns>
        Task DoStuff(string requestContextInfo,
            CancellationToken cToken = default(CancellationToken));

        /// <summary>
        /// Publish an <see cref="AEvent">AEvent</see> with <see cref="ExtendedPayload">BasicPayload</see>
        /// </summary>
        /// <param name="requestContextInfo"></param>
        /// <param name="cToken"></param>
        /// <returns></returns>
        Task DoMoreStuff(string requestContextInfo,
            CancellationToken cToken = default(CancellationToken));

        /// <summary>
        /// Publish an <see cref="IGenericEvent{CEvent}">IGenericPublisherEvent&lt;CEvent&gt;</see> with <see cref="BasicPayload">BasicPayload</see>
        /// </summary>
        /// <param name="requestContextInfo"></param>
        /// <param name="cToken"></param>
        /// <returns></returns>
        Task DoStuffWithATwist(string requestContextInfo,
            CancellationToken cToken = default(CancellationToken));
    }

    /// <summary>
    /// Sample service which publishes events
    /// <list type="bullet">
    /// <item>
    /// <description>Two <see cref="AEvent">AEvent</see> events with different payloads: <see cref="BasicPayload">BasicPayload</see> and <see cref="ExtendedPayload">ExtendedPayload</see> respectively</description>
    /// </item>
    /// <item>
    /// <description>one <see cref="IGenericEvent{CEvent}">IGenericPublisherEvent&lt;CEvent&gt;</see> event with <see cref="BasicPayload">BasicPayload</see></description>
    /// </item>
    /// </list>
    /// </summary>
    /// <remarks>Events are typically published while executing ops associated with a service's business logic</remarks>
    public class BusinessService : IBusinessService
    {
        private readonly IPublishEndpoint _publishEndpoint;

        private readonly ICloudEventWriter _ceWriter;

        private readonly ILogger<BusinessService> _logger;

        /// <summary>
        /// Ctor with a publish endpoint, event writer and logger
        /// </summary>
        /// <param name="publishEndpoint"></param>
        /// <param name="ceWriter"></param>
        /// <param name="logger"></param>
        public BusinessService(IPublishEndpoint publishEndpoint, ICloudEventWriter ceWriter, 
            ILogger<BusinessService> logger)
        {
            this._publishEndpoint = publishEndpoint;
            this._ceWriter = ceWriter;
            this._logger = logger;
        }

        private void LogException(string source, Exception ex)
        {
            var errDetails = ex.InnerException != null ?
                ex.InnerException.GetType().Name + " - " + ex.InnerException.Message :
                ex.StackTrace ?? "NA";

            _logger.LogError($"{source}: failed with exception '{ex.Message}', of type '{ex.GetType().Name}', details '{errDetails}'");
        }

        protected async Task Publish<TPayload,TEvent>(TPayload payload, string eventSubject = null,
            CancellationToken cToken = default(CancellationToken)) where TEvent : class, IEvent
        {
            await _publishEndpoint.PublishCloudEvent<TPayload, TEvent>(
                _ceWriter, payload, eventSubject: eventSubject,
                cToken);
        }

        #region IBusinessService implementation

        /// <summary>
        /// Publish an <see cref="AEvent">AEvent</see> with <see cref="BasicPayload">BasicPayload</see>
        /// </summary>
        /// <param name="requestContextInfo"></param>
        /// <param name="cToken"></param>
        /// <returns></returns>
        public async Task DoStuff(string requestContextInfo,
            CancellationToken cToken = default(CancellationToken))
        {
            try
            {
                var basicPayload = new BasicPayload()
                {
                    Id = 1000,
                    Description = "publisher payload",
                    Tags = new List<string>() { "tag0000", "tag1000", "tag2000", "tag3000" }
                };

                await Publish<BasicPayload, AEvent>(
                    basicPayload, requestContextInfo, cToken); // request context -> event subject

                _logger.LogInformation($"DoStuff: Published an AEvent with `{basicPayload.GetType().Name}` data; event context '{requestContextInfo}'");
            }
            catch( Exception ex)
            {
                // err
                LogException("DoStuff", ex);
            }
        }

        /// <summary>
        /// Publish an <see cref="AEvent">AEvent</see> with <see cref="ExtendedPayload">BasicPayload</see>
        /// </summary>
        /// <param name="requestContextInfo"></param>
        /// <param name="cToken"></param>
        /// <returns></returns>
        public async Task DoMoreStuff(string requestContextInfo,
            CancellationToken cToken = default(CancellationToken))
        {
            try
            {
                var extendedPayload = new ExtendedPayload()
                {
                    Id = 2000,
                    Description = "publisher payload, extended",
                    Tags = new List<string>() { "tag0000", "tag2000", "tag4000", "tag6000" },
                    Properties = new Dictionary<string, object>() { { "p0", "v0" }, { "p1", "v1" } }
                };

                await Publish<ExtendedPayload, AEvent>(
                    extendedPayload, requestContextInfo, cToken); // request context -> event subject

                _logger.LogInformation($"DoMoreStuff: Published an AEvent with `{extendedPayload.GetType().Name}` data; event context '{requestContextInfo}'");
            }
            catch(Exception ex)
            {
                // err
                LogException("DoMoreStuff", ex);
            }
        }

        /// <summary>
        /// Publish an <see cref="IGenericEvent{CEvent}">IGenericPublisherEvent&lt;CEvent&gt;</see> with <see cref="BasicPayload">BasicPayload</see>
        /// </summary>
        /// <param name="requestContextInfo"></param>
        /// <param name="cToken"></param>
        /// <returns></returns>
        public async Task DoStuffWithATwist(string requestContextInfo,
            CancellationToken cToken = default(CancellationToken))
        {
            try
            {
                var basicPayload = new BasicPayload()
                {
                    Id = 1000,
                    Description = "publisher payload sent with a generic event type",
                    Tags = new List<string>() { "generic-tag0000", "generic-tag1000", "generic-tag2000", "generic-tag3000" }
                };

                await Publish<BasicPayload, IGenericEvent<CEvent>>(
                     basicPayload, requestContextInfo, cToken ); // request context -> event subject

                _logger.LogInformation($"DoStuffWithATwist: Published an IGenericEvent<CEvent> with `{basicPayload.GetType().Name}` data; event context '{requestContextInfo}'");
            }
            catch (Exception ex)
            {
                // err
                LogException("DoStuffWithATwist", ex);
            }
        }

        #endregion
    }
}
