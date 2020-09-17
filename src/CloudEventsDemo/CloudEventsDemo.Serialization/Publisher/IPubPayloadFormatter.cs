namespace CloudEventsDemo.Serialization
{
    /// <summary>
    /// Publisher payload formatter interface
    /// </summary>
    public interface IPubPayloadFormatter
    {
        string SerializedContentType { get;  }
        object Serialize<T>(T pLoad);
    }
}
