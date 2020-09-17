using System;

namespace CloudEventsDemo.Serialization
{
    public class SubAppException : Exception
    {
        public SubAppException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
