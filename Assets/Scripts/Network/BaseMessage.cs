using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using System.Text;
using System.Net;

public enum MessageType
{
    HandShake = -1,
    Console = 0,
    Position = 1
}

public abstract class BaseMessage<PayloadType>
{
    protected PayloadType data;

    public static Action<PayloadType> OnDispatch;
    
    public abstract MessageType GetMessageType();
    public abstract byte[] Serialize();
    public abstract PayloadType Deserialize(byte[] message);
    public abstract PayloadType GetData();
}

public abstract class BaseOrderableMessage<PayloadType> : BaseMessage<PayloadType>
{
    protected static ulong LastMsgID = 0;

    protected ulong MsgID = 0;
    protected static Dictionary<PayloadType, ulong> lastExecutedMsgID = new Dictionary<PayloadType, ulong>();
    
    protected BaseOrderableMessage(byte[] msg)
    {
        MsgID = BitConverter.ToUInt64(msg,4);
    }
}

public class NetHandShake : BaseMessage<(long, int)>
{
    (long, int) data;
    public override (long, int) Deserialize(byte[] message)
    {
        (long, int) outData;

        outData.Item1 = BitConverter.ToInt64(message, 4);
        outData.Item2 = BitConverter.ToInt32(message, 12);

        return outData;
    }

    public override (long, int) GetData()
    {
        return data;
    }

    public override MessageType GetMessageType()
    {
       return MessageType.HandShake;
    }

    public override byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));

        outData.AddRange(BitConverter.GetBytes(data.Item1));
        outData.AddRange(BitConverter.GetBytes(data.Item2));


        return outData.ToArray();
    }
}

public class NetVector3 : BaseMessage<UnityEngine.Vector3>
{
    private static ulong lastMsgID = 0;
    private Vector3 data;

    public NetVector3(Vector3 data)
    {
        this.data = data;
    }

    public override Vector3 Deserialize(byte[] message)
    {
        Vector3 outData;

        outData.x = BitConverter.ToSingle(message, 8);
        outData.y = BitConverter.ToSingle(message, 12);
        outData.z = BitConverter.ToSingle(message, 16);

        return outData;
    }

    public override Vector3 GetData()
    {
        return data;
    }

    public override MessageType GetMessageType()
    {
        return MessageType.Position;
    }

    public override byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
        outData.AddRange(BitConverter.GetBytes(lastMsgID++));
        outData.AddRange(BitConverter.GetBytes(data.x));
        outData.AddRange(BitConverter.GetBytes(data.y));
        outData.AddRange(BitConverter.GetBytes(data.z));

        return outData.ToArray();
    }

    //Dictionary<Client,Dictionary<msgType,int>>
}

public class NetConsole : BaseMessage<String>
{
    private static ulong lastMsgID = 0;
    private string data;

    public NetConsole(byte[] dataToDeserialize)
    {
        data = Deserialize(dataToDeserialize);
    }

    public NetConsole(string data)
    {
        this.data = data;
    }

    public override MessageType GetMessageType() { return MessageType.Console; }

    public override byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
        outData.AddRange(BitConverter.GetBytes(data.Length));
        //outData.AddRange(BitConverter.GetBytes(lastMsgID++));
        outData.AddRange(Encoding.UTF8.GetBytes(data));
        
        return outData.ToArray();
    }

    public override string Deserialize(byte[] message)
    {
        int stringlenght = BitConverter.ToInt32(message, 4);
        return Encoding.UTF8.GetString(message,8,stringlenght);
    }

    public override string GetData()
    {
        return data;
    }
}