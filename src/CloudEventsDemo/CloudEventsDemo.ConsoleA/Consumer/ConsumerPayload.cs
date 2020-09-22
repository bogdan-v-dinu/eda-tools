using System.Collections.Generic;

namespace CloudEventsDemo.ConsoleA
{
    /// <summary>
    /// Sample consumer payload. 
    /// Matches publisher's <see cref="BasicPayload">basic payload</see>, without any contract sharing/coupling
    /// </summary>
    public class ConsumerPayload
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        public ConsumerPayload()
        {
            Tags = new List<string>();
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
        public List<string> Tags { get; set; }

    }
}
