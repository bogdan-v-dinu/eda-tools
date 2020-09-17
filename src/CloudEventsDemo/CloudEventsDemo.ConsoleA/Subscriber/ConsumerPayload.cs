using System;
using System.Collections.Generic;
using System.Text;

namespace CloudEventsDemo.ConsoleA
{
    public class ConsumerPayload
    {
        public ConsumerPayload()
        {
            Tags = new List<string>();
        }

        public int Id { get; set; }

        public string Description { get; set; }

        public List<string> Tags { get; set; }

    }
}
