using System;

[Serializable]
public class MessageBase 
{
    public string Name { get; set; }
    public bool Log { get; set; }

    public MessageBase()
    {
        object[] attrs = GetType().GetCustomAttributes(typeof(MessageAttribute), false);
        foreach (object attr in attrs)
        {
            var message = (MessageAttribute)attr;
            Name = message.Name;
            Log = message.Log;
            return;
        }
    }
}

