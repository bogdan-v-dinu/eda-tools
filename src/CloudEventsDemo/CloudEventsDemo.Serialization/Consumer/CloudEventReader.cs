using CloudNative.CloudEvents;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudEventsDemo.Serialization
{
    /// <summary>
    /// Reads from CloudEvent serialized envelopes
    /// </summary>
    public class CloudEventReader : ICloudEventReader
    {
        /// <summary>
        /// Payload formatters supported by this subscriber. Used to deserialize the actual payload found in the CloudEvent envelope
        /// </summary>
        protected List<IPayloadDeserializationFormatter> _pLoadFormatters;

        /// <summary>
        /// Mapping between declarative CloudEvents types (URNs) AND 
        /// </summary>
        protected Dictionary<string, Type> _typeMappings;

        /// <summary>
        /// CloudEvent formatter will always serialize the envelope as json, regardless of how the payload is serialized/formatted
        /// </summary>
        ICloudEventFormatter _cEventsFormatter;

        /// <summary>
        /// Logger interface
        /// </summary>
        ILogger<CloudEventReader> _logger;

        public CloudEventReader(Dictionary<string, Type> typeMappings, ILogger<CloudEventReader> logger)
        {
            #region Check arguments
            if (typeMappings == null || typeMappings.Count == 0)
            {
                throw new ArgumentNullException("typeMappings");
            }
            #endregion

            this._typeMappings = typeMappings;
            this._logger = logger;
            this._pLoadFormatters = new List<IPayloadDeserializationFormatter>() { new JsonPayloadDeserializationFormatter() };
            this._cEventsFormatter = new JsonEventFormatter();
        }

        #region ICloudEventReader implementation

        public List<IPayloadDeserializationFormatter> PayloadFormatters 
        {
            get { return _pLoadFormatters; }
        }

        public CloudEvent GetCloudEvent(byte[] cEventBytes)
        {
            CloudEvent cEvent = null;

            if (cEventBytes != null && cEventBytes.Length > 0)
            {
                try
                {
                    cEvent = _cEventsFormatter.DecodeStructuredEvent(cEventBytes, null);
                    // info
                    _logger.LogInformation($"GetCloudEvent: CloudEvents envelope specversion='{cEvent.SpecVersion}' type='{cEvent.Type}' source='{cEvent.Source.ToString()}' id='{cEvent.Id}' subject='{cEvent.Subject ?? "NA"}' content-type='{cEvent.DataContentType.MediaType}'");
                }
                catch (Exception ex)
                {
                    // err
                    _logger.LogError($"GetCloudEvent: failed with exception '{ex.Message}', of type '{ex.GetType().Name}'");
                    throw new CloudEventReaderException("GetCloudEvent", ex);
                }
            }
            else
            {
                // warn
                _logger.LogWarning("GetCloudEvent: specified byte array is empty");
            }

            return cEvent;
        }

        public object GetPayload(CloudEvent cEvent)
        {
            object pLoadObj = null;

            if (cEvent != null)
            {
                try
                {
                    if (!_typeMappings.ContainsKey(cEvent.Type.ToLower())) // type is a required envelope param
                    {
                        throw new ArgumentException($"Subscriber lacks a type mapping for '{cEvent.Type.ToLower()}'", "CloudEvent.Type");
                    }

                    var subscriberType = _typeMappings[cEvent.Type.ToLower()];
                    var pLoadContentType = cEvent.DataContentType.MediaType ?? "application/json";
                    var pLoadFormatter = _pLoadFormatters.FirstOrDefault(pf => pf.CanDeserialize(pLoadContentType));

                    if (pLoadFormatter == null )
                    {
                        throw new ArgumentException($"No subscriber formatter can deserialize a payload with content type '{pLoadContentType}'", "CloudEvent.DataContentType");
                    }

                    // debug/ max verbosity
                    _logger.LogDebug($"GetPayload: received CloudEvents Data as UTF8 string = '{cEvent.Data as string}'");

                    pLoadObj = pLoadFormatter.Deserialize(cEvent.Data, subscriberType, pLoadContentType);
                }
                catch (ArgumentException agEx)
                {
                    // warn
                    _logger.LogWarning($"GetPayload: cannot retrieve the payload from a CloudEvent due to unknown envelope params - '{agEx.Message}'");
                }
                catch (Exception ex)
                {
                    // err
                    _logger.LogError($"GetPayload: failed with exception '{ex.Message}', of type '{ex.GetType().Name}'");
                    throw new CloudEventReaderException("SubAdapter.GetCloudEvent", ex);
                }
            }
            else
            {
                // warn
                _logger.LogWarning("GetPayload: specified CloudEvent is empty");
            }

            return pLoadObj;
        }

        public object GetPayload(byte[] cEventBytes)
        {
            var cEvent = GetCloudEvent(cEventBytes);
            return cEvent != null ? GetPayload(cEvent) : null;
        }

        #endregion
    }
}
