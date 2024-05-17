using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NetworkManagerClient : NetworkManager
{
    public int clientId = 0; //  cliente
    public string playerName; //  cliente

    private void Awake()
    {
        _serviceLocator = ServiceLocator.Global;

        _serviceLocator.Register<NetworkManagerClient>(GetType(), this);
    }

    private void OnDisable()
    {
        NetConsole.OnDispatch -= OnDispatchNetCon;
        NetServerToClientHS.OnDispatch -= OnDispatchNetS2C;
    }

    public void StartClient(IPAddress ip, int port, string name) // cliente pero con mensaje para el servidor
    {
        this.port = port;
        this.ipAddress = ip;
        playerName = name;

        connection = new UdpConnection(ip, port, this);

        Player aux = new Player(name, -7);
        NetClientToServerHS nacho = new NetClientToServerHS(aux);
        SendToServer(nacho.Serialize());

        NetPing ping = new NetPing();
        SendToServer(ping.Serialize());

        NetConsole.OnDispatch += OnDispatchNetCon;
        NetServerToClientHS.OnDispatch += OnDispatchNetS2C;
    }

    public void SendToServer(byte[] data)
    {
        connection.Send(data);
    }

    protected void OnDispatchNetS2C(List<Player> obj)
    {
        players = obj;
    }

    public override void OnReceiveData(byte[] data, IPEndPoint ip)
    {
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
                        if (player.playerName == playerName)
                        {
                            clientId = player.playerID;
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
                    if (!CheckTimeDiference(DateTime.UtcNow))
                    {
                        SetLastRecivedPingTime(DateTime.UtcNow);
                        SendToServer(ping.Serialize());
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
}