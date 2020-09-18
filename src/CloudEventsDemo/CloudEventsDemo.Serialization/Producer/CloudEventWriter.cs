using CloudNative.CloudEvents;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;

namespace CloudEventsDemo.Serialization
{
    /// <summary>
    /// Generates serialized CloudEvent envelopes
    /// </summary>
    public class CloudEventWriter : ICloudEventWriter
    {
        /// <summary>
        /// Payload formatters supported by this publisher. Used to serialize the actual payload placed in a CloudEvent envelope 
        /// and to determine its underlying content type
        /// </summary>
        protected List<IPayloadSerializationFormatter> _pLoadFormatters;

        /// <summary>
        /// Mapper from .NET payload types to declarative CloudEvent types
        /// </summary>
        protected Func<string, string> _declarativeTypeMapper;

        /// <summary>
        /// Authority segment to be used in the declarative CloudEvent type URN 
        /// </summary>
        protected string _eventTypeUrnAuthority;

        /// <summary>
        /// Event source identifying the issuer within the Publisher realm
        /// </summary>
        protected Uri _eventSource;

        /// <summary>
        /// CloudEvent formatter will always deserialize the envelope from application/json, regardless of how the contained payload is serialized/formatted
        /// </summary>
        ICloudEventFormatter _cEventsFormatter;

        /// <summary>
        /// Logger interface
        /// </summary>
        ILogger<CloudEventWriter> _logger;

        public CloudEventWriter(ILogger<CloudEventWriter> logger) 
            : this("cloud-events", new Uri("http://cloud-events-demo.com"), logger)
        { 
        }

        public CloudEventWriter(string eventTypeUrnAuthority, Uri eventSource, ILogger<CloudEventWriter> logger)
        {
            #region Check arguments
            if (String.IsNullOrWhiteSpace(eventTypeUrnAuthority))
            {
                throw new ArgumentNullException("eventTypeUrnAuthority");
            }
            if (eventSource == null)
            {
                throw new ArgumentNullException("eventSource");
            }
            #endregion

            this._pLoadFormatters = new List<IPayloadSerializationFormatter>() { new JsonPayloadSerializationFormatter() };
            this._declarativeTypeMapper = Utils.ConvertPascalToKebabCase;
            this._eventTypeUrnAuthority = eventTypeUrnAuthority.ToLower();
            this._eventSource = eventSource;
            this._cEventsFormatter = new JsonEventFormatter();
            this._logger = logger;
        }

        #region ICloudEventWriter implementation

        public List<IPayloadSerializationFormatter> PayloadFormatters 
        {
            get
            {
                return _pLoadFormatters;
            }
        }

        public CloudEvent GetCloudEvent<T>(T pLoad, string pLoadId = null, string pLoadType = null, 
            string eventSubject = null, string targetContentType = null)
        {
            CloudEvent cEvent = null;

            if (pLoad != null)
            {
                try
                {
                    // param defaults
                    if (String.IsNullOrWhiteSpace(pLoadId))
                    {
                        pLoadId = Guid.NewGuid().ToString(); // could check for an Id property first, using reflection
                    }
                    if (String.IsNullOrWhiteSpace(pLoadType))
                    {
                        pLoadType = typeof(T).Name; // use custom formatting for generic type definitions
                    }
                    if (String.IsNullOrWhiteSpace(targetContentType))
                    {
                        targetContentType = "application/json"; // by default serialize as json
                    }

                    var declarativeType = System.Uri.EscapeUriString($"urn:{_eventTypeUrnAuthority}:{_declarativeTypeMapper(pLoadType)}");
                    
                    var pLoadFormatter = _pLoadFormatters.FirstOrDefault(pf => pf.SerializedContentType.Equals(targetContentType));
                    if (pLoadFormatter == null)
                    {
                        throw new ArgumentException($"No publisher formatter can serialize a payload with content type '{targetContentType}'", "targetContentType");
                    }

                    // create the CloudEvent instance
                    cEvent = new CloudEvent(type: declarativeType, source: _eventSource, id: pLoadId)
                    {
                        DataContentType = new ContentType(pLoadFormatter.SerializedContentType),
                        Data = pLoadFormatter.Serialize<T>(pLoad)
                    };
                    if (!String.IsNullOrWhiteSpace(eventSubject))
                    {
                        cEvent.Subject = eventSubject;
                    };

                    // info
                    _logger.LogInformation($"GetCloudEvent: created CloudEvents envelope specversion='{cEvent.SpecVersion}' type='{cEvent.Type}' source='{cEvent.Source.ToString()}' id='{cEvent.Id}' subject='{cEvent.Subject ?? "NA"}' content-type='{cEvent.DataContentType.MediaType}'");
                }
                catch (ArgumentException agEx)
                {
                    // warn
                    _logger.LogWarning($"GetCloudEvent: cannot add the payload into a CloudEvent - '{agEx.Message}'");
                }
                catch (Exception ex)
                {
                    // err
                    _logger.LogError($"GetCloudEvent: failed with exception '{ex.Message}', of type '{ex.GetType().Name}'");
                    throw new CloudEventWriterException("GetCloudEvent", ex);
                }
            }
            else
            {
                // warn
                _logger.LogWarning("GetCloudEvent: specified payload of type '{typeof(T)}' is empty");
            }
            
            return cEvent;
        }

        public byte[] GetBytes(CloudEvent cEvent)
        {
            byte[] cEventBytes = null;
            ContentType formatterContentType;

            if (cEvent != null)
            {
                try
                {
                    cEventBytes = _cEventsFormatter.EncodeStructuredEvent(cEvent, out formatterContentType); // no async 
                    // info
                    _logger.LogInformation($"GetBytes: generated {cEventBytes.Length} bytes with formatter content type {formatterContentType.MediaType}");
                    // debug/ max verbosity
                    _logger.LogDebug($"GetBytes: CloudEvent with envelope and full content {Environment.NewLine}{Encoding.UTF8.GetString(cEventBytes)}");
                }
                catch (Exception ex)
                {
                    // err
                    _logger.LogError($"GetBytes: failed with exception '{ex.Message}', of type '{ex.GetType().Name}'");
                    throw new CloudEventWriterException("GetBytes", ex);
                }
            }
            else
            {
                // warn
                _logger.LogWarning("GetBytes: specified CloudEvent is empty");
            }

            return cEventBytes;
        }

        public byte[] GetBytes<T>(T pLoad, string pLoadId = null, string pLoadType = null, 
            string eventSubject = null, string serializedContentType = null)
        {
            var cEvent = GetCloudEvent<T>(pLoad, pLoadId, pLoadType, eventSubject, serializedContentType);
            return cEvent != null ? GetBytes(cEvent) : null;
        }

        #endregion
    }
}
