# What

Lightweight lib built on top of `CloudNative.CloudEvents` 
 * Serializes & formats .NET types in a standard CloudEvents envelope
 * Deserializes standard CloudEvents envelopes to .NET types
 * Facilitates creation of self-describing payloads (\*1), enabling flexible type mappings on payload consumer's side

(\*1) Based on CloudEvents envelope/context information such as `type` and `datacontenttype`.

# Why

The [CloudEvents .NET SDK](https://github.com/cloudevents/sdk-csharp) states that:
"_The CloudEvent class is not meant to be used with object serializers like JSON.NET and does not have a default constructor to underline this._"

The lib is useful in scenarios where one requires the CloudEvents transport-agnostic event representation and formatters (JsonEventFormatter, AvroEventFormatter)
WITHOUT CloudEvents transport-specific bindings and clients.

Pros/Cons of such a setup are out-of scope for this wiki page.

#How

## Producer

1. The payload is formatted (a.k.a serialized) first using an `IPayloadSerializationFormatter` implementation 
2. The CloudEventWriter selects a formatter based on `dataContentType` param from client API
 * FTTB the lib includes just a Json formatter. 
 * others - XML formatter, binary ones - could be added in the future 
3. The formatter must return either "string", "Stream" or "byte[]", which gets assigned to `Data` param from the CloudEvent envelope.
4. CloudEventWriter uses its own formatter - an implementation of `CloudNative.CloudEvents.ICloudEventFormatter` - to serialize the CloudEvent envelope including payload 
 * When data is "Stream" or "byte[]", base64 encoding is applied by the CloudEvents formatter
 * FTTB only `CloudNative.CloudEvents.JsonEventFormatter` is used
 * `CloudNative.CloudEvents.AvroEventFormatter` could be added in the future

## Consumer

1. A CloudEvent is reconstructed from the byte array, using an implementation of `CloudNative.CloudEvents.ICloudEventFormatter`. FTTB only `CloudNative.CloudEvents.JsonEventFormatter` is used.
2. The CloudEventReader selects a payload formatter based on `datacontenttype` param from the cloud envelope
3. The payload is deserialized using the selected `IPayloadSerializationFormatter` implementation. FTTB the lib includes just a Json formatter.