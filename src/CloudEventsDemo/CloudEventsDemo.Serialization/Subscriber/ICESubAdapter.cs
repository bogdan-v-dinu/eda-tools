using CloudNative.CloudEvents;
using System.Collections.Generic;

namespace CloudEventsDemo.Serialization
{
    /// <summary>
    /// CloudEvent subscriber adapter interface
    /// </summary>
    public interface ICESubAdapter
    {
        List<ISubPayloadFormatter> PayloadFormatters { get;  }
        CloudEvent GetCloudEvent(byte[] cEventBytes);
        object GetPayload(CloudEvent cEvent);
        object GetPayload(byte[] cEventBytes);
    }
}
