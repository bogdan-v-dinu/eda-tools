using CloudEventsDemo.Contracts;
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
    /// Interface for a demo service which publishes events
    /// </summary>
    public interface IBusinessService
    {
        Task DoSomeBasicStuff(string requestContextInfo,
            CancellationToken cToken = default(CancellationToken));

        Task DoMoreStuff(string requestContextInfo,
            CancellationToken cToken = default(CancellationToken));
    }

    /// <summary>
    /// Publishes two <see cref="cref="IAEvent">IAEvent</see> events 
    /// with different payloads: <see cref="cref="BasicPayload">BasicPayload</see> and <see cref="cref="ExtendedPayload">ExtendedPayload</see> respectively
    /// </summary>
    /// <remarks>
    /// Events are typically published while executing ops associated with a service's business logic
    /// </remarks>
    public class BusinessService : IBusinessService
    {
        private readonly IPublishEndpoint _publishEndpoint;

        private readonly ICloudEventWriter _ceWriter;

        private readonly ILogger<BusinessService> _logger;

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

        #region IBusinessService implementation

        public async Task DoSomeBasicStuff(string requestContextInfo,
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

                await _publishEndpoint.Publish<AEvent>(new
                    {
                        EventData = _ceWriter.GetBytes<BasicPayload>(basicPayload,
                                            pLoadId: basicPayload.Id.ToString(), eventSubject: requestContextInfo)
                    },
                    cToken);

                _logger.LogInformation($"DoSomeBasicStuff: Published an AEvent with `{basicPayload.GetType().Name}` data; event context '{requestContextInfo}'");
            }
            catch( Exception ex)
            {
                // err
                LogException("DoSomeBasicStuff", ex);
            }
        }

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

                await _publishEndpoint.Publish<AEvent>(new
                    {
                        EventData = _ceWriter.GetBytes<ExtendedPayload>(extendedPayload,
                                    pLoadId: extendedPayload.Id.ToString(), eventSubject: requestContextInfo)
                    },
                    cToken);

                _logger.LogInformation($"DoMoreStuff: Published an AEvent with `{extendedPayload.GetType().Name}` data; event context '{requestContextInfo}'");
            }
            catch(Exception ex)
            {
                // err
                LogException("DoMoreStuff", ex);
            }
        }

        #endregion
    }
}
