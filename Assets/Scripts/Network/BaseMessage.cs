using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

// Hacer los mensajes de ping que se tiene que mandar siempre
// El primero en mandar el mensaje es el cliente y no se vuelve a mandar hasta obtener la respuesta del servver
// Es un tipo de mensaje unico


public enum MessageType
{
    HandShake = -3,
    ServerToClientHS = -1,
    ClientToServerHS = -2,
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

public struct Player
{
    public string playerName;
    public int playerID;

    public Player(string playerName,int playerID)
    {
        this.playerName = playerName;
        this.playerID = playerID;
    }
}

public class NetServerToClientHS : BaseMessage<Player>
{
    public static Action<List<Player>> OnDispatch;
    
    private Player data;
    
    public override MessageType GetMessageType()
    {
        return MessageType.ServerToClientHS;
    }

    public override byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
        
        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream())
        {
            bf.Serialize(ms, data);
            outData.AddRange(ms.ToArray());
        }
        
        
        return outData.ToArray();
    }

    public override Player Deserialize(byte[] message)
    {
        MemoryStream memStream = new MemoryStream();
        BinaryFormatter binForm = new BinaryFormatter();
        memStream.Write(message, 4, message.Length);
        memStream.Seek(4, SeekOrigin.Begin);
        Player obj = (Player) binForm.Deserialize(memStream);

        return obj;
    }

    public override Player GetData()
    {
        return data;
    }
}

public class ClientToServerHS : BaseMessage<string>
{
    private string data;
    
    public override MessageType GetMessageType()
    {
        return MessageType.ClientToServerHS;
    }

    public override byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
        outData.AddRange(BitConverter.GetBytes(data.Length));
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


public class NetHandShake : BaseMessage<int>
{
    int data;

    public void SetClientID(int clientID)
    {
        data = clientID;
    }

    public NetHandShake(byte[] dataToDeserialize)
    {
        data = Deserialize(dataToDeserialize);
    }
    
    public NetHandShake()
    {
        data = -7;
    }

    public override int Deserialize(byte[] message)
    {
        return BitConverter.ToInt32(message, 4);;
    }

    public override int GetData()
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

        outData.AddRange(BitConverter.GetBytes(data));
        
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