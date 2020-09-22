using MassTransit;
using CloudEventsDemo.Contracts;
using CloudEventsDemo.Serialization;

namespace CloudEventsDemo.MessageBrokerExtensions
{
    /// <summary>
    /// MassTransit consumer extensions for CloudEvent(s)
    /// </summary>
    public static class ConsumerExtensions
    {
        /// <summary>
        /// Reconstructs a CloudEvent from the <see cref="IEvent"/> consumer context and returns the payload found in the CloudEvent envelope
        /// </summary>
        /// <param name="context"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static object GetCloudEventsPayload(this ConsumeContext<IEvent> context, ICloudEventReader reader) 
        {
            object result = null;

            if ( reader != null )
            {
                result = reader.GetPayload(context.Message.EventData);
            }

            return result;
        }

        /// <summary>
        /// Reconstructs a CloudEvent from the <see cref="IGenericEvent{T}"/> consumer context and returns the payload found in the CloudEvent envelope
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static object GetCloudEventsPayload<T>(this ConsumeContext<IGenericEvent<T>> context, ICloudEventReader reader)
        {
            object result = null;

            if (reader != null)
            {
                result = reader.GetPayload(context.Message.EventData);
            }

            return result;
        }
    }
}
