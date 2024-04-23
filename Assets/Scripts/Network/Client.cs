using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Client
{
    public float timeStamp;
    public int id;
    public string name;
    public IPEndPoint ipEndPoint;

    private List<Player> players = new List<Player>();

    public Client(IPEndPoint ipEndPoint, int id, float timeStamp)
    {
        this.timeStamp = timeStamp;
        this.id = id;
        this.ipEndPoint = ipEndPoint;
        this.name = "";
        
        NetConsole.OnDispatch += OnDispatchNetCon;
        NetVector3.OnDispatch += OnDispatchNetVec3;
        NetHandShake.OnDispatch += OnDispatchNetHS;
        NetServerToClientHS.OnDispatch += OnDispatchNetS2C;
    }
    
    public Client(IPEndPoint ipEndPoint, int id, float timeStamp,string name)
    {
        this.timeStamp = timeStamp;
        this.id = id;
        this.ipEndPoint = ipEndPoint;
        this.name = name;
        
        NetConsole.OnDispatch += OnDispatchNetCon;
        NetVector3.OnDispatch += OnDispatchNetVec3;
        NetHandShake.OnDispatch += OnDispatchNetHS;
        NetServerToClientHS.OnDispatch += OnDispatchNetS2C;
    }

    private void OnDispatchNetS2C(List<Player> obj)
    {
        players = obj;
    }

    private void OnDispatchNetHS(int obj)
    {
        this.id = obj;
    }

    private void OnDispatchNetVec3(Vector3 obj)
    {
        Debug.Log("OnDispatch (Vector3 obj)");
    }

    private void OnDispatchNetCon(string obj)
    {
        Debug.Log("OnDispatch (string obj)");
        ChatScreen.Instance.ReceiveConsoleMessage(obj);
    }
}