# CloudEvents Initiative

## Introduction
 * _**Home**_: https://cloudevents.io/
 * _**Coordinated by**_: Cloud Native Computing Foundation - https://www.cncf.io/
 * _**Maturity**_: "early adopters" stage, with 1+ years since graduating from "sandbox" to "incubator" project - https://www.cncf.io/projects/
 * _**SDK support**_: Go, JavaScript, Java, C#, Ruby and Python
 * _**Github**_: 
	- Schema Specification: https://github.com/cloudevents/spec/blob/v1.0/spec.md
	- .NET SDK https://github.com/cloudevents/sdk-csharp. At v1.3, mostly authored by [Clemens Vasters](https://vasters.com/), a Distributed Systems and Messaging Software Architect an co-chair of AMQP Technical Committee in OASIS
 * _**Licensing**_: Free for commercial user under Apache 2.0, for details see https://tldrlegal.com/license/apache-license-2.0-(apache-2.0)
 
## Summary
 * _**What**_: Vendor-neutral open specification for defining the format of event data. 
 
   It includes the following
	- Event and messaging terminology
	- Event schema requirements - data and required/optional/extension context attributes - using key words from [RFC 2119](https://tools.ietf.org/html/rfc2119) (MUST, SHOULD, MAY, etc)
	- Supported data types
	- 2x Event formats, which specify how to serialize a CloudEvent to a sequence of bytes. Implementations MUST suport the JSON format and MAY also support Apache Avro
	- Recommendation for size, security and privacy
   
   It does NOT include
	- Protocol bindings, or mapping of events to messages from various industry standard protocols - AMQP, MQTT, etc
	- Specific data encryption requirements 
 * _**Why**_: Aims to standardize and provide interoperability across services, platforms and systems.
 * _**How**_: CloudEvents project includes SDKs compliant with the specification, which DO implement protocol bindings for various messaging protocols. 
   Noteworthy, the main CloudEvent class in the SDK is not meant to be used directly with object serializers like JSON.NET (no default constructor). Instead, one has to explicitly leverage the SDK's serializers.
 
## Details

In CloudEvents terminology an "_event_" consists in 
 * an "_occurrence_" representing data/payload
 * a "_context_" representing metadata/event attributes
 
An "_event format_" describes how to serialize an "_event_" to a sequence of bytes and a "_protocol binding_" describes how to send/receive events using a given protocol (ex mapping event attributes to protocol specific message headers). 

| specversion 1.0 | Type        | Description                                                 | Required | Extension |
| --------------- | ----------- | ----------------------------------------------------------- |:--------:|:---------:|
|        |        | _**STANDARD ATTRIBUTES**_                                                 |   |  |
| `specversion` | string | "1.0".                                                             | Y |  |
| `type` | string | Event type, defined by producer.                                          | Y |  |
| `source` | URI-reference | URI incl. info about event source or publisher.                  | Y |  |
| `id` | string | Identifies the event (UUID or event counter). `source` + `id` is unique.    | Y |  |
| `datacontenttype` | string | MIME type, defaults to "application/json".                     |   |  |
| `dataschema` | URI | Schema that `data` adheres to.                                         |   |  |
| `subject` | string | Additional qualifier for `source`, based on an internal sub-structure. |   |  |
| `time` | timestamp | Actual time of event occurrence, represented as s RFC3339 timestamp.   |   |  |
|        |        | _**EXTENSION ATTRIBUTES**_                                                |   |  |
| _`{ext-attrib}`_ | _{type}_ | Serialized independently of the event data.<br/>Designed to be inspected at destination w-out having to deserialize the entire event data. |   | Y |
|        |        | _**EVENT DATA**_                                                          |   |  |
| `data` | binary | base64 encoded payload                                                    |	  |  |


```json
	// CloudEvents event 
	{
		"specversion" : "1.0",
		"type" : "com.github.pull.create",
		"source" : "https://github.com/cloudevents/spec/pull",
		"subject" : "123",
		"id" : "A234-1234-1234",
		"time" : "2018-04-05T17:31:00Z",
		"comexampleextension1" : "value",
		"comexampleothervalue" : 5,
		"datacontenttype" : "text/xml",
		"data" : "<much wow=\"xml\"/>"
	}
```

_**Note**: CloudEvents spec is neither an IETF nor an OASIS standard._

## Comparison to other event schemas

### MassTransit 

References
 * Transports: https://masstransit-project.com/usage/transports/
 * Message Envelope: https://masstransit-project.com/architecture/interoperability.html

| Attribute Name  | .NET Type   | Description                                                                              | Required    | CloudEvents Mapping |
| --------------- | ----------- | ---------------------------------------------------------------------------------------- |:-----------:| ------------------- |
| messageType | string[] | Should be URNs with the following custom format `urn:message:Namespace:TypeName`                | Y           | `type` |
| sourceAddress | string | Source identifier. All "_address_" types should be URIs                                         |             | `source`? |
| destinationAddress | string | Likely identifies the type of broker and destination queue or topic. <br/>All "_address_" types should be URIs	| Y           | NA | 
| responseAddress | string | Definition NA. All "_address_" types should be URIs                                           |             | NA | 
| messageId | string | All "_id_" types should be URIs                                                                     |             | `id` |
| correlationId | string | Likely used for operational purposes (instrumentation). All "_id_" types should be URIs         |             | NA |
| conversationId | string | Likely used for message sequences. All "_id_" types should be URIs                             |             | `sequence` (known extension) |
| initiatorId | string | Definition NA. All "_id_" types should be URIs                                                    |             | NA |
| requestId | string | Definition NA. All "_id_" types should be URIs                                                      |             | NA |
| sentTime | DateTime? | Sent time                                                                                         |             | `time` (_opt_) |
| expirationTime | DateTime? | Event/message expiration time                                                               |             | NA |
| host | HostInfo | An internal data type, should be represented as                                                        |             | NA |
| headers | IDictionary<string,object> | Additional headers                                                                | Y (**\*1**) | _`{ext-attrib}`_ (collection of) |
| message | object | Actual payload                                                                                        | Y           |    |

_**Notes**_: 
 * MassTransit defines its own proprietary message envelope (event envelope in CloudEvents), which encapsulates the built-in message headers and payload
 * Some constraints defined by MassTransit for its attributes/headers are not mirrored in other event schema, complicating implementation of a potential adapter (other schema->MassTransit)
 * (**\*1**) "_headers_" field is required but the "_headers_" collection can be empty

### Azure Event Grid default event schema 

Reference: https://docs.microsoft.com/en-us/azure/event-grid/event-schema

| metaDataVersion 1 | Type        | Description                                                                              | Required | CloudEvents Mapping |
| ----------------- | ----------- | ---------------------------------------------------------------------------------------- |:--------:| ------------------- |
| `eventType` | string | A registered event type for the event source, ex "_Microsoft.Storage.BlobCreated_"                  | Y | `type` | 
| `subject` | string | Publisher-defined path to the event subject, ex an absolute or relative path to a blob/blob container | Y | `subject` (_opt_) | 
| `id` | string | Unique event identifier                                                                                    | Y | `id` | 
| `eventTime` | string | Event generation timestamp, expressed as UTC                                                        | Y | `time` (_opt_) | 
| `topic` | string | Populated by EventGrid with resource path to the event source, _**read-only**_                          |   | `source` |
| `dataVersion` | string | Schema version of the data object, defined by publisher                                           |   | `dataschema` |
| `metaDataVersion` | string |  Event Grid schema version, currently only "1" supported                                      |   | NA |
| `data` | object | Actual payload from publisher, which can have additional publisher-specific properties                   |   | `data` |

Notes: 
 * Unlike CloudEvents, Event Grid defines no data types; all properties except for `data` are strings. 
 * Event Grid operates with arrays of messages
 * Azure Event Grid natively supports events in the JSON implementation of CloudEvents v1.0, likely by means of an adapter.

```json
	// Azure Event Grid default event schema
	[
	  {
		"eventType": "Microsoft.Storage.BlobCreated",
		"subject": "/blobServices/default/containers/oc2d2817345i200097container/blobs/oc2d2817345i20002296blob",
		"id": "831e1650-001e-001b-66ab-eeb76e069631",
		"eventTime": "2017-06-26T18:41:00.9584103Z",
		"topic": "/subscriptions/{id}/resourceGroups/Storage/providers/Microsoft.Storage/storageAccounts/{acc}",
		"data":{
		  ...
		},
		"dataVersion": "",
		"metadataVersion": "1"
	  }
	]
``` 
