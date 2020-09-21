using System;
using System.Collections.Generic;
using System.Text;

namespace CloudEventsDemo.Contracts
{
    /// <summary>
    /// Generic publisher event with a byte array as event data
    /// </summary>
    public interface IPublisherEvent
    {
        byte[] EventData { get; set; }
    }
}
