using CloudNative.CloudEvents;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudEventsDemo.Serialization
{
    /// <summary>
    /// CloudEvent subscriber adapter 
    /// </summary>
    /// <remarks>todo: add a logger</remarks>
    public class SubAdapter : ICESubAdapter
    {
        /// <summary>
        /// Payload formatters supported by this subscriber. Used to deserialize the actual payload found in the CloudEvent envelope
        /// </summary>
        protected List<ISubPayloadFormatter> _pLoadFormatters;

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
        ILogger<SubAdapter> _logger;

        public SubAdapter(Dictionary<string, Type> typeMappings, ILogger<SubAdapter> logger)
        {
            #region Check arguments
            if (typeMappings == null || typeMappings.Count == 0)
            {
                throw new ArgumentNullException("typeMappings");
            }
            #endregion

            this._typeMappings = typeMappings;
            this._logger = logger;
            this._pLoadFormatters = new List<ISubPayloadFormatter>() { new SubJsonPayloadFormatter() };
            this._cEventsFormatter = new JsonEventFormatter();
        }

        #region ICESubAdapter

        public List<ISubPayloadFormatter> PayloadFormatters 
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
                    _logger.LogInformation($"SubAdapter.GetCloudEvent: CloudEvents envelope specversion='{cEvent.SpecVersion}' type='{cEvent.Type}' source='{cEvent.Source.ToString()}' id='{cEvent.Id}' subject='{cEvent.Subject ?? "NA"}' content-type='{cEvent.DataContentType.MediaType}'");
                    //Console.WriteLine($"SubAdapter.GetCloudEvent: CloudEvents envelope specversion='{cEvent.SpecVersion}' type='{cEvent.Type}' source='{cEvent.Source.ToString()}' id='{cEvent.Id}' subject='{cEvent.Subject ?? "NA"}' content-type='{cEvent.DataContentType.MediaType}'");
                }
                catch (Exception ex)
                {
                    // err
                    _logger.LogError($"SubAdapter.GetCloudEvent: failed with exception '{ex.Message}', of type '{ex.GetType().Name}'");
                    //Console.WriteLine($"SubAdapter.GetCloudEvent: failed with exception '{ex.Message}', of type '{ex.GetType().Name}'");
                    throw new SubAppException("SubAdapter.GetCloudEvent", ex);
                }
            }
            else
            {
                // warn
                _logger.LogWarning("SubAdapter.GetCloudEvent: specified byte array is empty");
                //Console.WriteLine("SubAdapter.GetCloudEvent: specified byte array is empty"); 
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
                    _logger.LogDebug($"SubAdapter.GetPayload: received CloudEvents Data as UTF8 string = '{cEvent.Data as string}'");
                    //Console.WriteLine($"SubAdapter.GetPayload: received CloudEvents Data as UTF8 string = '{cEvent.Data as string}'");

                    pLoadObj = pLoadFormatter.Deserialize(cEvent.Data, subscriberType, pLoadContentType);
                }
                catch (ArgumentException agEx)
                {
                    // warn
                    _logger.LogWarning($"SubAdapter.GetPayload: cannot retrieve the payload from a CloudEvent due to unknown envelope params - '{agEx.Message}'");
                    //Console.WriteLine($"SubAdapter.GetPayload: cannot retrieve the payload from a CloudEvent due to unknown envelope params - '{agEx.Message}'");
                }
                catch (Exception ex)
                {
                    // err
                    _logger.LogError($"SubAdapter.GetPayload: failed with exception '{ex.Message}', of type '{ex.GetType().Name}'");
                    //Console.WriteLine($"SubAdapter.GetPayload: failed with exception '{ex.Message}', of type '{ex.GetType().Name}'");
                    throw new SubAppException("SubAdapter.GetCloudEvent", ex);
                }
            }
            else
            {
                // warn
                _logger.LogWarning("SubAdapter.GetPayload: specified CloudEvent is empty");
                //Console.WriteLine("SubAdapter.GetPayload: specified CloudEvent is empty");
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
