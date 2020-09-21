using System;
using System.Collections.Generic;
using System.Text;

namespace CloudEventsDemo.Contracts
{
    /// <summary>
    /// Universal publisher event, with event data stored as a byte array.
    /// Inherit this to define domain-specific events for each publisher.
    /// </summary>
    public interface IEvent
    {
        byte[] EventData { get; set; }
    }
}
