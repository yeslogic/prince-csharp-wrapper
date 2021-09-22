namespace PrinceXML.Wrapper.Events
{
    public enum MessageType
    {
        ERR,
        WRN,
        INF,
        DBG,
        OUT
    }

    public interface PrinceEvents
    {
        void OnMessage(MessageType msgType, string msgLocation, string msgText);

        void OnDataMessage(string name, string value);
    }
}
