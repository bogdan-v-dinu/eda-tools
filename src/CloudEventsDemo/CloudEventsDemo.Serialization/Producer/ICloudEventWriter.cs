using CloudNative.CloudEvents;
using System.Collections.Generic;

namespace CloudEventsDemo.Serialization
{
    /// <summary>
    /// Interface for generating/writing serialized CloudEvent envelopes
    /// </summary>
    public interface ICloudEventWriter
    {
        List<IPayloadSerializationFormatter> PayloadFormatters { get; }
        CloudEvent GetCloudEvent<T>(T pLoad, string pLoadId = null, string pLoadType = null, 
            string eventSubject = null, string dataContentType = null);
        byte[] GetBytes(CloudEvent cEvent);
        byte[] GetBytes<T>(T pLoad, string pLoadId = null, string pLoadType = null, 
            string eventSubject = null, string targetContentType = null);
    }
}
