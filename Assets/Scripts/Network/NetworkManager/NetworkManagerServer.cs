using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NetworkManagerServer : NetworkManager
{
    private readonly Dictionary<int, Client> clients = new Dictionary<int, Client>(); // servidor 
    private readonly Dictionary<IPEndPoint, int> ipToId = new Dictionary<IPEndPoint, int>(); // servidor

    private int clientId;
    


    public void StartServer(int port)
    {
        _serviceLocator = ServiceLocator.Global;
        
        this.port = port;
        connection = new UdpConnection(port, this);

        IsServer = true;
     
        _serviceLocator.Register<NetworkManagerServer>(GetType(), this);
        NetConsole.OnDispatch += OnDispatchNetCon;
    }
    
    private void OnDestroy()
    {
        NetConsole.OnDispatch -= OnDispatchNetCon;
    }

    public void AddClient(IPEndPoint ip, Player lean) // servidor 
    {
        if (!ipToId.ContainsKey(ip))
        {
            lean.playerID = clientId;
            ipToId[ip] = clientId;

            clients.Add(clientId, new Client(ip, lean.playerID, Time.realtimeSinceStartup));
            players.Add(lean);
            clientId++;
        }
        
        foreach (Player player in players)
        {
            Debug.Log("Player Name : " + player.playerName + " Player ID : " + player.playerID);
        }
    }

    public void RemoveClient(IPEndPoint ip) // servidor 
    {
        if (ipToId.ContainsKey(ip))
        {
            Debug.Log("Removing client: " + ip.Address);
            clients.Remove(ipToId[ip]);
            RemovePlayer(ipToId[ip]);
        }
    }

    public void SendToClient(byte[] data, IPEndPoint ip)
    {
        connection.Send(data, ip);
    }

    public void Broadcast(byte[] data)
    {
        using (var iterator = clients.GetEnumerator())
        {
            while (iterator.MoveNext())
            {
                connection.Send(data, iterator.Current.Value.ipEndPoint);
            }
        }
    }

    public override void OnReceiveData(byte[] data, IPEndPoint ip)
    {
        HandleServerMessage(data, ip);
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
                    Broadcast(con.Serialize());
                    NetConsole.OnDispatch?.Invoke(con.GetData());
                    Debug.Log(nameof(MessageType.Console) + ": The message is ok");
                }
                else
                    Debug.Log(nameof(MessageType.Console) + ": The message is corrupt");

                break;
            case MessageType.Position:
                break;
            case MessageType.ClientToServerHS:
                NetClientToServerHS c2s = new NetClientToServerHS(data);
                if (c2s.CheckMessage(data))
                {
                    AddClient(ep, c2s.GetData());
                    NetServerToClientHS s2c = new NetServerToClientHS(GetCurrentPlayers());
                    Broadcast(s2c.Serialize());
                    Debug.Log(nameof(MessageType.ClientToServerHS) + ": The message is ok");
                }
                else
                {
                    Debug.Log(nameof(MessageType.ClientToServerHS) + ": The message is corrupt");
                }

                break;
            case MessageType.Ping:
                NetPing ping = new NetPing();
                if (ping.CheckMessage(data))
                {
                    if (CheckTimeDiference(DateTime.UtcNow))
                    {
                        SetLastRecivedPingTime(DateTime.UtcNow);
                        SendToClient(ping.Serialize(), ep);
                    }
                    else
                    {
                        // NetworkManager.Instance.RemoveClient(ep);
                    }

                    Debug.Log(nameof(MessageType.Ping) + ": The message is ok");
                }
                else
                {
                    Debug.Log(nameof(MessageType.Ping) + ": The message is corrupt");
                }

                break;
        }
    }
    public override void Update()
    {
        base.Update();
        if (!CheckTimeDiference(DateTime.UtcNow))
            Application.Quit();
    }
}