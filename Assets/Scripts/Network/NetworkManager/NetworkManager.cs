using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NetworkManager : MonoBehaviour , IReceiveData
{
    protected ServiceLocator _serviceLocator;
    public static bool IsServer {get;protected set; }

    protected void OnDispatchNetCon(string obj)
    {
        Debug.Log("OnDispatch (string obj)");
        _serviceLocator.Get(out ChatScreen chatScreen);
        chatScreen.ReceiveConsoleMessage(obj);
    }

    public IPAddress ipAddress // servidor y cliente
    {
        get; protected set;
    }

    public int port // servidor y cliente
    {
        get; protected set;
    }

    protected int TimeOut = 5; // servidor y cliente

    protected Action<byte[], IPEndPoint> OnReceiveEvent; // servidor y cliente

    protected UdpConnection connection; // servidor y cliente
    
    protected List<Player> players = new List<Player>(); // servidor y cliente


    protected DateTime lastTimeReceivedPing = DateTime.UtcNow; // servidor y cliente

    protected void RemovePlayer(int id) // cliente y servidor
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

    public void SetLastRecivedPingTime(DateTime currentTime) { lastTimeReceivedPing = currentTime; }

    public bool CheckTimeDiference(DateTime currentTime)
    {
        float diference = lastTimeReceivedPing.Second - currentTime.Second;
        Debug.Log("Time Diference : " + diference);
        if (diference > TimeOut)
        {
            return false;
        }
        return true;
    }

    public virtual void Update()
    {
        // Flush the data in main thread
        if (connection != null)
            connection.FlushReceiveData();
    }
}
