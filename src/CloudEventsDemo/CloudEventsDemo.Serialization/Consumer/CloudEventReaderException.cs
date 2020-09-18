using System;

namespace CloudEventsDemo.Serialization
{
    public class CloudEventReaderException : Exception
    {
        public CloudEventReaderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
