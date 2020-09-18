namespace CloudEventsDemo.Serialization
{
    /// <summary>
    /// Payload serialization formatter interface
    /// </summary>
    public interface IPayloadSerializationFormatter
    {
        string SerializedContentType { get;  }
        object Serialize<T>(T pLoad);
    }
}
