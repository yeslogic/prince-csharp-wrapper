namespace PrinceXML.Wrapper.Events
{
    /// <summary>The type of message received from Prince.</summary>
    public enum MessageType
    {
        /// <summary>Error message.</summary>
        ERR,
        /// <summary>Warning message.</summary>
        WRN,
        /// <summary>Information message.</summary>
        INF,
        /// <summary>Debug message.</summary>
        DBG,
        /// <summary>Console output from <c>console.log()</c>.</summary>
        OUT
    }

    /// <summary>Can be used to receive messages from Prince.</summary>
    public interface PrinceEvents
    {
        /// <summary>
        /// This method will be called when a message is received from Prince.
        /// </summary>
        /// <param name="msgType">The type of message.</param>
        /// <param name="msgLocation">The name of the file that the message refers to.</param>
        /// <param name="msgText">The text of the message.</param>
        void OnMessage(MessageType msgType, string msgLocation, string msgText);

        /// <summary>
        /// This method will be called when a data message is received from Prince
        /// via the use of <c>Log.data("name", "value")</c>.
        /// </summary>
        /// <param name="name">The name of the data message.</param>
        /// <param name="value">The value of the data message.</param>
        void OnDataMessage(string name, string value);
    }
}
