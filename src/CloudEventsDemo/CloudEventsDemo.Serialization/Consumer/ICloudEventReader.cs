using CloudNative.CloudEvents;
using System.Collections.Generic;

namespace CloudEventsDemo.Serialization
{
    /// <summary>
    /// Interface for reading serialized CloudEvent envelopes
    /// </summary>
    public interface ICloudEventReader
    {
        List<IPayloadDeserializationFormatter> PayloadFormatters { get;  }
        CloudEvent GetCloudEvent(byte[] cEventBytes);
        object GetPayload(CloudEvent cEvent);
        object GetPayload(byte[] cEventBytes);
    }
}
