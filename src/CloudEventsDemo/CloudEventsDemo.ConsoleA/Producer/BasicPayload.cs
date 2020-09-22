using System;
using System.Collections.Generic;
using System.Text;

namespace CloudEventsDemo.ConsoleA
{
    /// <summary>
    /// Sample producer payload
    /// </summary>
    public class BasicPayload
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        public BasicPayload() : this(null)
        {
        }

        /// <summary>
        /// Ctor with a collection of tags
        /// </summary>
        /// <param name="tags"></param>
        public BasicPayload(List<string> tags)
        {
            Tags = (tags != null) ? new List<string>(tags) : new List<string>();
        }

        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Collection of tags
        /// </summary>
        public List<string> Tags { get; set;  }
    }
}
