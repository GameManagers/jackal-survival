using System;
[Serializable]
public class MessageData : MessageBase
{
    public Object Body { get; set; }

    public MessageData(string messageName, MessageBase messageBody)
    {
        Name = messageName;
        Body = messageBody;
    }
}