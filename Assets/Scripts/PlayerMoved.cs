using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using shared;

public class PlayerMoved : ASerializable
{
    public float[] pos = new float[3];
    public int ID = -1;


    public PlayerMoved() { }
    public PlayerMoved(Vector3 pos, int iD)
    {
        this.pos[0] = pos.x;
        this.pos[1] = pos.y;
        this.pos[2] = pos.z;
        ID = iD;
    }

    public override void Deserialize(Packet pPacket)
    {
        ID = pPacket.ReadInt();
        for (int i = 0; i < 3; i++)
        {
            pos[i] = pPacket.ReadFloat();
        }
    }

    public override void Serialize(Packet pPacket)
    {
        pPacket.Write(ID);
        for (int i = 0; i < 3; i++)
        {
            pPacket.Write(pos[i]);
        }
    }
}
