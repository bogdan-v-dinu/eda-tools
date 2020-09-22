using System;
using System.Collections.Generic;
using System.Text;

namespace CloudEventsDemo.ConsoleA
{
    /// <summary>
    /// Variation/extension of the sample consumer payload
    /// </summary>
    class ExtendedPayload : BasicPayload
    {
        public ExtendedPayload() : this(null)
        {
        }

        public ExtendedPayload(Dictionary<string, object> properties) : this(null, properties)
        {
        }

        public ExtendedPayload(List<string> tags, Dictionary<string, object> properties) : base(tags)
        {
            // should really clone here
            this.Properties = (properties != null) ?
                new Dictionary<string, object>(properties) : new Dictionary<string, object>();
        }

        public Dictionary<string, object> Properties { get; set; }
    }
}
