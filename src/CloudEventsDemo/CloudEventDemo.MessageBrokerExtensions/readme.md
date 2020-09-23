# What

MassTransit helper and extension methods to facilitate CloudEvent(s) handling.

# Why

Send and receive payloads in a CloudEvent(s) envelope using MassTransit. 
Use the so called "_**structured-mode message**_" defined in the [CloudEvents Specification](https://github.com/cloudevents/spec/blob/master/spec.md).

# How

## Producer

Use `ProducerExtensions` to publish CloudEvents with an `IPublishEndpoint`. 
FTTB there is just one extension method, the list could be extended in the future.

Likely similar extension methods will be needed for `IRequestClient`.

## Consumer

Use `ConsumerExtensions` to reconstruct a CloudEvent from a consumer context and to return the payload found in the CloudEvent envelope.
