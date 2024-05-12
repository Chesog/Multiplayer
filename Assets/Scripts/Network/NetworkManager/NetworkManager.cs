using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NetworkManager : MonoBehaviour , IReceiveData
{
    private ServiceLocator _serviceLocator;
    private void Awake()
    {
        _serviceLocator = ServiceLocator.global;
        _serviceLocator.Register<NetworkManager>(GetType(), this);
    }

    private void OnEnable()
    {
        NetConsole.OnDispatch += OnDispatchNetCon;
        NetServerToClientHS.OnDispatch += OnDispatchNetS2C;
    }
    private void OnDisable()
    {
        NetConsole.OnDispatch -= OnDispatchNetCon;
    }
    private void OnDispatchNetCon(string obj)
    {
        Debug.Log("OnDispatch (string obj)");
        _serviceLocator.Get(out ChatScreen chatScreen);
        chatScreen.ReceiveConsoleMessage(obj);
    }
    
    private void OnDispatchNetS2C(List<Player> obj)
    {
        players = obj;
    }

    public IPAddress ipAddress
    {
        get; private set;
    }

    public int port
    {
        get; private set;
    }

    public bool isServer
    {
        get; private set;
    }

    public int TimeOut = 5;

    public Action<byte[], IPEndPoint> OnReceiveEvent;

    private UdpConnection connection;

    private readonly Dictionary<int, Client> clients = new Dictionary<int, Client>();
    private readonly Dictionary<IPEndPoint, int> ipToId = new Dictionary<IPEndPoint, int>();
    private List<Player> players = new List<Player>();

    public int clientId = 0;
    public string playerName;
    private DateTime lastTimeReceivedPing = DateTime.UtcNow;

    public void StartServer(int port)
    {
        isServer = true;
        this.port = port;
        connection = new UdpConnection(port, this);
    }

    public void StartClient(IPAddress ip, int port,string name)
    {
        isServer = false;

        this.port = port;
        this.ipAddress = ip;
        playerName = name;

        connection = new UdpConnection(ip, port, this);

        Player aux = new Player(name,-7);
        NetClientToServerHS nacho = new NetClientToServerHS(aux);
        SendToServer(nacho.Serialize());

        NetPing ping = new NetPing();
        SendToServer(ping.Serialize());
    }

    public void AddClient(IPEndPoint ip,Player lean)
    {
        if (!ipToId.ContainsKey(ip))
        {
            lean.playerID = clientId;
            ipToId[ip] = clientId;

            clients.Add(clientId, new Client(ip, lean.playerID, Time.realtimeSinceStartup));
            players.Add(lean);
            clientId++;
        }
    }

    public void RemoveClient(IPEndPoint ip)
    {
        if (ipToId.ContainsKey(ip))
        {
            Debug.Log("Removing client: " + ip.Address);
            clients.Remove(ipToId[ip]);
            RemovePlayer(ipToId[ip]);
        }
    }

    private void RemovePlayer(int id)
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

    public void OnReceiveData(byte[] data, IPEndPoint ip)
    {
        if (OnReceiveEvent != null)
            OnReceiveEvent.Invoke(data, ip);
    }

    public void SendToServer(byte[] data)
    {
        connection.Send(data);
    }

    public void SendToClient(byte[] data, IPEndPoint ip)
    {
       connection.Send(data,ip);
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

    public List<Player> GetCurrentPlayers() { return players; }

    public void SetLastRecivedPingTime(DateTime currentTime) { lastTimeReceivedPing = currentTime; }

    public bool CheckTimeDiference(DateTime currentTime)
    {
        float diference = currentTime.Second - lastTimeReceivedPing.Second;
        if (diference > TimeOut)
        {
            return true;
        }
        return false;
    }

    void Update()
    {
        // Flush the data in main thread
        if (connection != null)
            connection.FlushReceiveData();
        
        
    }
}
