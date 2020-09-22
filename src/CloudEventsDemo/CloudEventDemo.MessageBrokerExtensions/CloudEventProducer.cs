using CloudEventsDemo.Serialization;
using CloudEventsDemo.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudEventsDemo.MessageBrokerExtensions
{
    /// <summary>
    /// MassTransit producer helpers for CloudEvent(s)
    /// </summary>
    public class CloudEventProducer
    {
        internal class EventContract : IEvent
        {
            public byte[] EventData { get; set; }
        }

        internal class GenericEventContract<T> : IGenericEvent<T>
        {
            public byte[] EventData { get; set; }
        }

        /// <summary>
        /// Adds the formatted payload to a CloudEvent envelope and returns an <see cref="IEvent"/> 
        /// with the serialized CloudEvent as event data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="writer"></param>
        /// <param name="payload"></param>
        /// <param name="eventSubject"></param>
        /// <returns></returns>
        public static IEvent GetEvent<T>(ICloudEventWriter writer, T payload, string eventSubject = null)
        {
            if (String.IsNullOrWhiteSpace(eventSubject))
            {
                eventSubject = String.Empty;
            }

            return new EventContract
            {
                EventData = writer.GetBytes<T>(
                    payload, eventSubject: eventSubject)
            };
        }

        /// <summary>
        /// Adds the formatted payload to a CloudEvent envelope and returns an <see cref="IGenericEvent{TEvent}"/> 
        /// with the serialized CloudEvent as event data
        /// </summary>
        /// <typeparam name="TPayload"></typeparam>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="writer"></param>
        /// <param name="payload"></param>
        /// <param name="eventSubject"></param>
        /// <returns></returns>
        public static IGenericEvent<TEvent> GetEvent<TPayload,TEvent>(ICloudEventWriter writer, TPayload payload, string eventSubject = null)
        {
            if (String.IsNullOrWhiteSpace(eventSubject))
            {
                eventSubject = String.Empty;
            }

            return new GenericEventContract<TEvent>
            {
                EventData = writer.GetBytes<TPayload>(
                    payload, eventSubject: eventSubject)
            };
        }

    }
}
