using System;

namespace CloudEventsDemo.Serialization
{
    public class PubAppException : Exception
    {
        public PubAppException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
