using System.Collections.Generic;

namespace CloudEventsDemo.ConsoleA
{
    /// <summary>
    /// Matches publisher's <see cref="BasicPayload">basic payload</see>, without any contract sharing/coupling
    /// </summary>
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
