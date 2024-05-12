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
    private NetworkManager _networkManager;

    protected void Awake()
    {
        _serviceLocator = ServiceLocator.global;
        _serviceLocator.Get(out NetworkManager manager);
        _networkManager = manager;
        connectBtn.onClick.AddListener(OnConnectBtnClick);
        startServerBtn.onClick.AddListener(OnStartServerBtnClick);
    }
    
    void OnConnectBtnClick()
    {
        IPAddress ipAddress = IPAddress.Parse(addressInputField.text);
        int port = System.Convert.ToInt32(portInputField.text);
        string playerName = nameInputField.text;
        
        _networkManager.StartClient(ipAddress, port,playerName);
        
        SwitchToChatScreen();
    }

    void OnStartServerBtnClick()
    {
        int port = System.Convert.ToInt32(portInputField.text);
        _networkManager.StartServer(port);
        SwitchToChatScreen();
    }

    void SwitchToChatScreen()
    {
        _serviceLocator.Get(out ChatScreen chatScreen);
        chatScreen.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
