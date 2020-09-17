using System;

namespace CloudEventsDemo.Serialization
{
    /// <summary>
    /// Subscriber payload formatter interface
    /// </summary>
    public interface ISubPayloadFormatter
    {
        bool CanDeserialize(string contentType);
        object Deserialize(object pLoad, Type pType, string contentType);
    }
}
