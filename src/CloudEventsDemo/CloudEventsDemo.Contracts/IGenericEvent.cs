using System;
using System.Collections.Generic;
using System.Text;

namespace CloudEventsDemo.Contracts
{
    /// <summary>
    /// Generic publisher event, with event data stored as a byte array.
    /// Use with domain-specific events as generic parameter.
    /// </summary>
    public interface IGenericEvent<T>
    {
        byte[] EventData { get; set; }
    }
}
