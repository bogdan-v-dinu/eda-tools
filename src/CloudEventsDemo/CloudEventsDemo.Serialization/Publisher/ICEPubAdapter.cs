using CloudNative.CloudEvents;
using System.Collections.Generic;

namespace CloudEventsDemo.Serialization
{
    /// <summary>
    /// CloudEvent publisher adapter interface
    /// </summary>
    public interface ICEPubAdapter
    {
        List<IPubPayloadFormatter> PayloadFormatters { get; }
        CloudEvent GetCloudEvent<T>(T pLoad, string pLoadId = null, string pLoadType = null, 
            string eventSubject = null, string targetContentType = null);
        byte[] GetBytes(CloudEvent cEvent);
        byte[] GetBytes<T>(T pLoad, string pLoadId = null, string pLoadType = null, 
            string eventSubject = null, string targetContentType = null);
    }
}
