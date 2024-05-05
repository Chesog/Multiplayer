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
            case MessageType.Console:
                NetConsole con = new NetConsole(message);
                NetConsole.OnDispatch?.Invoke(con.GetData());
                break;
            case MessageType.Position:
                //OnRecievePositionMessage?.Invoke();
                break;
            case MessageType.ServerToClientHS:
                NetServerToClientHS s2c = new NetServerToClientHS(message);
                //Chekear si mi nombre esta en la lista , si no esta volver a mandar un handshake 
                foreach (var player in s2c.GetData())
                {
                    Debug.Log("Player Name : " + player.playerName);
                    Debug.Log("Player ID : " + player.playerID);
                    if (player.playerName == NetworkManager.Instance.playerName)
                    {
                        NetworkManager.Instance.clientId = player.playerID;
                    }
                }
                NetServerToClientHS.OnDispatch.Invoke(s2c.GetData());
                break;
        }
    }

    public void HandleServerMessage(byte[] data, IPEndPoint ep)
    {
        MessageType temp = (MessageType)BitConverter.ToInt32(data, 0);
        
        switch (temp)
        {
            case MessageType.Console:
                NetConsole con = new NetConsole(data);
                NetworkManager.Instance.Broadcast(con.Serialize());
                NetConsole.OnDispatch?.Invoke(con.GetData());
                //ChatScreen.Instance.ReceiveConsoleMessage(con.GetData());
                break;
            case MessageType.Position:
                break;
            case MessageType.ClientToServerHS:
                NetClientToServerHS c2s = new NetClientToServerHS(data);
                NetworkManager.Instance.AddClient(ep,c2s.GetData());
                NetServerToClientHS s2c = new NetServerToClientHS(NetworkManager.Instance.GetCurrentPlayers());
                NetworkManager.Instance.Broadcast(s2c.Serialize());
                break;
        }
    }
}
