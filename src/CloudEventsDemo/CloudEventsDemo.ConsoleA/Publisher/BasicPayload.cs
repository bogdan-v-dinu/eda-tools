using System;
using System.Collections.Generic;
using System.Text;

namespace CloudEventsDemo.ConsoleA
{
    public class BasicPayload
    {
        public BasicPayload() : this(null)
        {
        }

        public BasicPayload(List<string> tags)
        {
            Tags = (tags != null) ? new List<string>(tags) : new List<string>();
        }


        public int Id { get; set; }
        
        public string Description { get; set; }

        public List<string> Tags { get; set;  }
    }
}
