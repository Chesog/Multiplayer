using System;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.AI;

public class MessageController : MonoBehaviourSingleton<MessageController>
{
    private ServiceLocator _serviceLocator;
    private NetworkManager _networkManager;
    private void Awake()
    {
        _serviceLocator = ServiceLocator.global;
        _serviceLocator.Register<MessageController>(GetType(), this);
    }

    private void OnEnable()
    {
        _serviceLocator.Get(out NetworkManager manager);
        _networkManager = manager;
        _networkManager.OnReceiveEvent += OnReceiveDataEvent;
    }

    private void OnDisable()
    {
        _serviceLocator.Get(out NetworkManager manager);
        _networkManager.OnReceiveEvent -= OnReceiveDataEvent;
    }

    private void OnReceiveDataEvent(byte[] data, IPEndPoint ep)
    {
        if (_networkManager.isServer)
            HandleServerMessage(data, ep);
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
                if (con.CheckMessage(message))
                {
                    NetConsole.OnDispatch?.Invoke(con.GetData());
                    Debug.Log(nameof(NetConsole) + ": The message is ok");
                }
                else
                    Debug.Log(nameof(NetConsole) + ": The message is corrupt");

                break;
            case MessageType.Position:
                //OnRecievePositionMessage?.Invoke();
                break;
            case MessageType.ServerToClientHS:
                NetServerToClientHS s2c = new NetServerToClientHS(message);
                if (s2c.CheckMessage(message))
                {
                    //Chekear si mi nombre esta en la lista , si no esta volver a mandar un handshake 
                    foreach (var player in s2c.GetData())
                    {
                        Debug.Log("Player Name : " + player.playerName);
                        Debug.Log("Player ID : " + player.playerID);
                        if (player.playerName == _networkManager.playerName)
                        {
                            _networkManager.clientId = player.playerID;
                        }
                    }

                    NetServerToClientHS.OnDispatch.Invoke(s2c.GetData());
                    Debug.Log(nameof(NetServerToClientHS) + ": The message is ok");
                }
                else
                {
                    Debug.Log(nameof(NetServerToClientHS) + ": The message is corrupt");
                }
                break;
            case MessageType.Ping:
                NetPing ping = new NetPing();
                if (ping.CheckMessage(message))
                {
                    if (!_networkManager.CheckTimeDiference(DateTime.UtcNow))
                    {
                        _networkManager.SetLastRecivedPingTime(DateTime.UtcNow);
                        _networkManager.SendToServer(ping.Serialize());
                    }
                    else
                    {
                        //NetworkManager.Instance.RemoveClient(ep);
                    }
                    Debug.Log(nameof(NetClientToServerHS) + ": The message is ok");
                }
                else
                {
                    Debug.Log(nameof(NetClientToServerHS) + ": The message is corrupt");
                }
               
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
                if (con.CheckMessage(data))
                {
                    _networkManager.Broadcast(con.Serialize());
                    NetConsole.OnDispatch?.Invoke(con.GetData());
                    Debug.Log(nameof(NetConsole) + ": The message is ok");
                }
                else
                    Debug.Log(nameof(NetConsole) + ": The message is corrupt");

                break;
            case MessageType.Position:
                break;
            case MessageType.ClientToServerHS:
                NetClientToServerHS c2s = new NetClientToServerHS(data);
                if (c2s.CheckMessage(data))
                {
                    _networkManager.AddClient(ep, c2s.GetData());
                    NetServerToClientHS s2c = new NetServerToClientHS(_networkManager.GetCurrentPlayers());
                    _networkManager.Broadcast(s2c.Serialize());
                    Debug.Log(nameof(NetClientToServerHS) + ": The message is ok");
                }
                else
                {
                    Debug.Log(nameof(NetClientToServerHS) + ": The message is corrupt");
                }
                break;
            case MessageType.Ping:
                NetPing ping = new NetPing();
                if (ping.CheckMessage(data))
                {
                    if (!_networkManager.CheckTimeDiference(DateTime.UtcNow))
                    {
                        _networkManager.SetLastRecivedPingTime(DateTime.UtcNow);
                        _networkManager.SendToClient(ping.Serialize(),ep);
                    }
                    else
                    {
                       // NetworkManager.Instance.RemoveClient(ep);
                    }
                    Debug.Log(nameof(NetClientToServerHS) + ": The message is ok");
                }
                else
                {
                    Debug.Log(nameof(NetClientToServerHS) + ": The message is corrupt");
                }
               
                break;
        }
    }
}