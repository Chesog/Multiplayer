using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NetworkManager : MonoBehaviour , IReceiveData
{
    public int port { get; protected set; }
    public IPAddress ipAddress { get; protected set; }
    public static bool IsServer {get;protected set; }
    protected ServiceLocator _serviceLocator;
    protected void OnDispatchNetCon(string obj)
    {
        Debug.Log("OnDispatch (string obj)");
        _serviceLocator.Get(out ChatScreen chatScreen);
        chatScreen.ReceiveConsoleMessage(obj);
    }
    protected int TimeOut = 15;
    protected Action<byte[], IPEndPoint> OnReceiveEvent;
    protected UdpConnection connection;
    protected List<Player> players = new List<Player>();
    
    protected void RemovePlayer(int id)
    {
        Player aux = new Player();
        foreach (var player in players)
        {
            if (player.playerID == id)
                aux = player;
        }

        if (players.Contains(aux))
            players.Remove(aux);

    }

    public virtual void OnReceiveData(byte[] data, IPEndPoint ip)
    {
        if (OnReceiveEvent != null)
            OnReceiveEvent.Invoke(data, ip);
    }

    public List<Player> GetCurrentPlayers() { return players; }
    
    public virtual void Update()
    {
        // Flush the data in main thread
        if (connection != null)
            connection.FlushReceiveData();
    }
}
