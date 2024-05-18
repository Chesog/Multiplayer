using System;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class ChatScreen : MonoBehaviour
{
    public Text messages;
    public InputField inputMessage;

    private ServiceLocator _serviceLocator;
    private NetworkManagerServer _networkManagerServer = new NetworkManagerServer();
    private NetworkManagerClient _networkManagerClient = new NetworkManagerClient();

    protected void Start()
    {
        _serviceLocator = ServiceLocator.Global;
        _serviceLocator.Register<ChatScreen>(GetType(), this);
        this.gameObject.SetActive(false);
    }

    public void InitChatScreen(bool isServer)
    {
        if (isServer)
        {
            _serviceLocator.Get(out NetworkManagerServer server);
            if (server != null)
                _networkManagerServer = server;
        }
        else
        {
            _serviceLocator.Get(out NetworkManagerClient clietn);
            if (clietn != null)
                _networkManagerClient = clietn;
        }
        inputMessage.onEndEdit.AddListener(OnEndEdit);
    }

    public void ReceiveConsoleMessage(string obj)
    {
        messages.text += obj + System.Environment.NewLine;
        Debug.Log(obj);
    }

    void OnEndEdit(string str)
    {
        if (!string.IsNullOrEmpty(inputMessage.text))
        {
            NetConsole temp = new NetConsole(inputMessage.text);
            if (NetworkManager.IsServer)
            {
                _networkManagerServer.HandleServerMessage(temp.Serialize());
            }
            else
                _networkManagerClient.SendToServer(temp.Serialize());

            inputMessage.ActivateInputField();
            inputMessage.Select();
            inputMessage.text = "";
        }
    }
}