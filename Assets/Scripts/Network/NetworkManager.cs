using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NetworkManager : MonoBehaviourSingleton<NetworkManager>, IReceiveData
{
    private void OnEnable()
    {
        NetHandShake.OnDispatch += OnDispatch;
    }

    private void OnDisable()
    {
        NetHandShake.OnDispatch -= OnDispatch;
    }

    private void OnDispatch( int obj)
    {
        clientId = obj;
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

    public int TimeOut = 30;

    public Action<byte[], IPEndPoint> OnReceiveEvent;

    private UdpConnection connection;

    private readonly Dictionary<int, Client> clients = new Dictionary<int, Client>();
    private readonly Dictionary<IPEndPoint, int> ipToId = new Dictionary<IPEndPoint, int>();
    public List<Player> players;

    public int clientId = 0;
    public string playerName;

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

    void RemoveClient(IPEndPoint ip)
    {
        if (ipToId.ContainsKey(ip))
        {
            Debug.Log("Removing client: " + ip.Address);
            clients.Remove(ipToId[ip]);
        }
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

    void Update()
    {
        // Flush the data in main thread
        if (connection != null)
            connection.FlushReceiveData();
    }
}
