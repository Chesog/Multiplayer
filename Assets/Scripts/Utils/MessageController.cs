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
        {
            NetworkManager.Instance.Broadcast(data);
        }
        
        HandleMessage(data);
    }

    public void HandleMessage(byte[] message)
    {
        MessageType temp = (MessageType)BitConverter.ToInt32(message, 0);
        
        switch (temp)
        {
            case MessageType.HandShake:
                break;
            case MessageType.Console:
                NetConsole a = new NetConsole(message);
                NetConsole.OnDispatch.Invoke(a.GetData());
                break;
            case MessageType.Position:
                //OnRecievePositionMessage?.Invoke();
                break;
        }
        
    }
}
