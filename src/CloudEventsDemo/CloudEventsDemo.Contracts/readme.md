# What

Producer events encapsulating an arbitrary serialized type as byte array.

# Why

Facilitates a separation between event type and event data, thus reducing coupling between producers and consumers.
In this scenario the event contracts library - shared between producers and consumers - will contain exclusively marker interfaces and/or classes.

# How

Producers creating their specific events library can either
 * Reference the library and create marker events which implement `IEvent` 
 * Create marker events used as generic type parameter for `IGenericEvent`


 _**Note**_: Without any shared contract for event data, producers and consumers have to 
  * Adopt a common, self-describing representation of that data
  * Use consistently the above representation for type mappings