using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using shared;

public class ChatMessage : ASerializable
{
    public string message;
    public ChatMessage(string message)
    {
        this.message = message;
    }
    public ChatMessage() { }

    public override void Deserialize(Packet pPacket)
    {
        message = pPacket.ReadString();
    }

    public override void Serialize(Packet pPacket)
    {
        pPacket.Write(message);
    }
}
