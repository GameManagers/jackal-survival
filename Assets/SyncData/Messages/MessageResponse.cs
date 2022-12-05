using System;
using System.Net.Sockets;
using UniRx;
[Serializable]
public class MessageResponse : MessageBase
{
    public int Status { get; set; }
    public string Msg { get; set; }

    public Object Body { get; set; }

    public MessageError Error { get; set; }

    public virtual void Execute() { }

    public bool IsSuccess { get { return Status == 1; } }

}

