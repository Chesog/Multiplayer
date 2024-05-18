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

    protected void Start()
    {
        _serviceLocator = ServiceLocator.Global;
        connectBtn.onClick.AddListener(OnConnectBtnClick);
        startServerBtn.onClick.AddListener(OnStartServerBtnClick);
    }
    
    void OnConnectBtnClick()
    {
        IPAddress ipAddress = IPAddress.Parse(addressInputField.text);
        int port = System.Convert.ToInt32(portInputField.text);
        string playerName = nameInputField.text;

        _networkClient = _serviceLocator.gameObject.AddComponent<NetworkManagerClient>();
        
        _networkClient.StartClient(ipAddress, port,playerName);
        _serviceLocator.Get(out NetworkManagerClient client);
        _networkClient = client;
        
        _serviceLocator.Get(out ChatScreen chatScreen);
        chatScreen.InitChatScreen(false);
        
        SwitchToChatScreen();
    }

    void OnStartServerBtnClick()
    {
        int port = Convert.ToInt32(portInputField.text);
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        
        //_networkClient = _serviceLocator.gameObject.AddComponent<NetworkManagerClient>();
        _networkServer = _serviceLocator.gameObject.AddComponent<NetworkManagerServer>();
        
        _networkServer.StartServer(port);
        _serviceLocator.Get(out NetworkManagerServer server);
        _networkServer = server;
        
        //_networkClient.StartClient(ipAddress, port,"Server");
        //_serviceLocator.Get(out NetworkManagerClient client);
        //_networkClient = client;
        
        _serviceLocator.Get(out ChatScreen chatScreen);
        chatScreen.InitChatScreen(true);
        
        SwitchToChatScreen();
    }

    void SwitchToChatScreen()
    {
        _serviceLocator.Get(out ChatScreen chatScreen);
        chatScreen.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
