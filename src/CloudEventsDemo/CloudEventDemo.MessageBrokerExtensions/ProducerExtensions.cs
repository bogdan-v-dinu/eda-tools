﻿using CloudEventsDemo.Contracts;
using CloudEventsDemo.Serialization;
using MassTransit;
using System.Threading;
using System.Threading.Tasks;

namespace CloudEventsDemo.MessageBrokerExtensions
{
    /// <summary>
    /// MassTransit producer extensions for CloudEvent(s)
    /// </summary>
    public static class ProducerExtensions
    {
        #region IPublishEndpoint extensions

        // add extension methods matching all referenced MassTransit.IPublishEndpoint.Publish signatures

        /// <summary>
        /// Publish a CloudEvent of type {TEvent} with the specified payload and (optionally) event subject
        /// </summary>
        /// <typeparam name="TPayload"></typeparam>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="publishEndpoint"></param>
        /// <param name="writer"></param>
        /// <param name="payload"></param>
        /// <param name="eventSubject"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task PublishCloudEvent<TPayload,TEvent>(this IPublishEndpoint publishEndpoint,
            ICloudEventWriter writer, TPayload payload, string eventSubject = null, 
            CancellationToken cancellationToken = default(CancellationToken)) where TEvent : class, IEvent
        {
            await publishEndpoint.Publish<TEvent>(
                CloudEventProducer.GetEvent<TPayload>(writer, payload, eventSubject), 
                cancellationToken);
        }

        /// <summary>
        /// Publish a CloudEvent of type <see cref="IGenericEvent{TEvent}"/> with the specified payload and (optionally) event subject
        /// </summary>
        /// <typeparam name="TPayload"></typeparam>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="publishEndpoint"></param>
        /// <param name="writer"></param>
        /// <param name="payload"></param>
        /// <param name="eventSubject"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task PublishGenericCloudEvent<TPayload, TEvent>(this IPublishEndpoint publishEndpoint,
            ICloudEventWriter writer, TPayload payload, string eventSubject = null,
            CancellationToken cancellationToken = default(CancellationToken)) where TEvent : class, IEvent
        {
            await publishEndpoint.Publish<TEvent>(
                CloudEventProducer.GetEvent<TPayload, TEvent>(writer, payload, eventSubject),
                cancellationToken);
        }

        #endregion
    }
}
