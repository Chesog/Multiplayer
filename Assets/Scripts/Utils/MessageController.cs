using System;
using System.Net;
using UnityEngine;

public class MessageController : MonoBehaviourSingleton<MessageController>
{
    private void OnEnable()
    {
        NetworkManager.Instance.OnReceiveEvent += OnReceiveDataEvent;
    }

    private void OnDisable()
    {
        NetworkManager.Instance.OnReceiveEvent -= OnReceiveDataEvent;
    }

    private void OnReceiveDataEvent(byte[] data, IPEndPoint ep)
    {
       
        if (NetworkManager.Instance.isServer)
            HandleServerMessage(data,ep);
        else
            HandleMessage(data);
    }

    public void HandleMessage(byte[] message)
    {
        MessageType temp = (MessageType)BitConverter.ToInt32(message, 0);
        
        switch (temp)
        {
            case MessageType.HandShake:
                NetHandShake aux = new NetHandShake(message);
                if (aux.GetData() != -7)
                {
                    NetworkManager.Instance.clientId = aux.GetData();
                    Debug.Log("client ID = " + NetworkManager.Instance.clientId );
                }
                break;
            case MessageType.Console:
                NetConsole con = new NetConsole(message);
                NetConsole.OnDispatch?.Invoke(con.GetData());
                break;
            case MessageType.Position:
                //OnRecievePositionMessage?.Invoke();
                break;
        }
    }

    public void HandleServerMessage(byte[] data, IPEndPoint ep)
    {
        MessageType temp = (MessageType)BitConverter.ToInt32(data, 0);
        
        switch (temp)
        {
            case MessageType.HandShake:
                NetHandShake aux = new NetHandShake();
                aux.SetClientID(NetworkManager.Instance.AddClient(ep));
                NetworkManager.Instance.SendToClient(aux.Serialize(),ep);
                break;
            case MessageType.Console:
                NetConsole con = new NetConsole(data);
                NetworkManager.Instance.Broadcast(con.Serialize());
                ChatScreen.Instance.ReceiveConsoleMessage(con.GetData());
                break;
            case MessageType.Position:
                break;
        }
    }
}
