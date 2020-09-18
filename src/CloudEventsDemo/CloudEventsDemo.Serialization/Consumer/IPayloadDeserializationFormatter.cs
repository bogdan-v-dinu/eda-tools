using System;

namespace CloudEventsDemo.Serialization
{
    /// <summary>
    /// Payload deserialization formatter interface
    /// </summary>
    public interface IPayloadDeserializationFormatter
    {
        bool CanDeserialize(string contentType);
        object Deserialize(object pLoad, Type pType, string contentType);
    }
}
