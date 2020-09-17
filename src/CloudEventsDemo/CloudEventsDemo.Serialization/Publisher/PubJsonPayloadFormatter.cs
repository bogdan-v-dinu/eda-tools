using Newtonsoft.Json;
using System.Net.Mime;

namespace CloudEventsDemo.Serialization
{
    /// <summary>
    /// Formatter able to serialize payloads with "application/json" content type
    /// </summary>
    public class PubJsonPayloadFormatter : IPubPayloadFormatter
    {
        #region IPubPayloadFormatter region
        
        public string SerializedContentType 
        {
            get
            {
                return MediaTypeNames.Application.Json;
            }
        }

        public object Serialize<T>(T pLoad)
        {
            return JsonConvert.SerializeObject(pLoad);
        }

        #endregion
    }
}
