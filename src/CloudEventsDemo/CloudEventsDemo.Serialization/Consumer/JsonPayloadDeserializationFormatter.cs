using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Mime;

namespace CloudEventsDemo.Serialization
{
    /// <summary>
    /// Formatter able to deserialize payloads with "application/json" content type
    /// </summary>
    public class JsonPayloadDeserializationFormatter : IPayloadDeserializationFormatter
    {
        /// <summary>
        /// Supported content types
        /// </summary>
        protected List<string> _contentTypes;
        
        public JsonPayloadDeserializationFormatter()
        {
            _contentTypes = new List<string>() { MediaTypeNames.Application.Json };
        }

        #region IPayloadDeserializationFormatter implementation

        public bool CanDeserialize(string contentType)
        {
            return !String.IsNullOrWhiteSpace(contentType) ? _contentTypes.Contains(contentType) : false;
        }

        public object Deserialize(object pLoad, Type pType, string contentType)
        {
            #region Check arguments

            if ( pLoad == null && String.IsNullOrWhiteSpace(pLoad as string) )
            { 
                throw new ArgumentNullException("pLoad");
            }
            if ( pType == null )
            {
                throw new ArgumentNullException("pType");
            }
            if ( String.IsNullOrWhiteSpace(contentType) )
            {
                throw new ArgumentNullException("contentType");
            }
            if ( !_contentTypes.Contains(contentType) )
            {
                throw new ArgumentException($"Formatter '{GetType().Name}' does not support '{contentType}' content type", "contentType");
            }

            #endregion

            var pLoadStr = pLoad as string;
            return JsonConvert.DeserializeObject(pLoad as string, pType);
        }

        #endregion
    }
}
