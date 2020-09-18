using System;

namespace CloudEventsDemo.Serialization
{
    public class CloudEventWriterException : Exception
    {
        public CloudEventWriterException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
