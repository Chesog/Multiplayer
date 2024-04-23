﻿using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Client
{
    public float timeStamp;
    public int id;
    public IPEndPoint ipEndPoint;

    private readonly Dictionary<int, Client> clients = new Dictionary<int, Client>();

    public Client(IPEndPoint ipEndPoint, int id, float timeStamp)
    {
        this.timeStamp = timeStamp;
        this.id = id;
        this.ipEndPoint = ipEndPoint;
        
        NetConsole.OnDispatch += OnDispatch;
        NetVector3.OnDispatch += OnDispatch;
        NetHandShake.OnDispatch += OnDispatch;
    }

    private void OnDispatch(int obj)
    {
        this.id = obj;
    }

    private void OnDispatch(Vector3 obj)
    {
        Debug.Log("OnDispatch (Vector3 obj)");
    }

    private void OnDispatch(string obj)
    {
        Debug.Log("OnDispatch (string obj)");
        ChatScreen.Instance.ReceiveConsoleMessage(obj);
    }
}

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

    public int clientId = 0;

    public void StartServer(int port)
    {
        isServer = true;
        this.port = port;
        connection = new UdpConnection(port, this);
    }

    public void StartClient(IPAddress ip, int port)
    {
        isServer = false;

        this.port = port;
        this.ipAddress = ip;

        connection = new UdpConnection(ip, port, this);

        NetHandShake nacho = new NetHandShake();
        SendToServer(nacho.Serialize());
    }

    public int AddClient(IPEndPoint ip)
    {
        if (!ipToId.ContainsKey(ip))
        {
            int id = clientId;
            ipToId[ip] = clientId;

            clients.Add(clientId, new Client(ip, id, Time.realtimeSinceStartup));
            clientId++;
        }
        return ipToId[ip];
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
        AddClient(ip);

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
