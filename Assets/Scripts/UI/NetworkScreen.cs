using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;

public class NetworkScreen : MonoBehaviourSingleton<NetworkScreen>
{
    public Button connectBtn;
    public Button startServerBtn;
    public InputField portInputField;
    public InputField addressInputField;
    public InputField nameInputField;
    
    private ServiceLocator _serviceLocator;
    private NetworkManagerServer _networkServer;
    private NetworkManagerClient _networkClient;

    protected void Awake()
    {
        _serviceLocator = ServiceLocator.global;
        _serviceLocator.Get(out NetworkManagerServer server);
        _networkServer = server;
        
        _serviceLocator.Get(out NetworkManagerClient client);
        _networkClient = client;
        
        connectBtn.onClick.AddListener(OnConnectBtnClick);
        startServerBtn.onClick.AddListener(OnStartServerBtnClick);
    }
    
    void OnConnectBtnClick()
    {
        IPAddress ipAddress = IPAddress.Parse(addressInputField.text);
        int port = System.Convert.ToInt32(portInputField.text);
        string playerName = nameInputField.text;
        
        _networkClient.StartClient(ipAddress, port,playerName);
        
        SwitchToChatScreen();
    }

    void OnStartServerBtnClick()
    {
        int port = System.Convert.ToInt32(portInputField.text);
        _networkServer.StartServer(port);
        SwitchToChatScreen();
    }

    void SwitchToChatScreen()
    {
        _serviceLocator.Get(out ChatScreen chatScreen);
        chatScreen.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
