      using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class ChatScreen : MonoBehaviour
{
    public Text messages;
    public InputField inputMessage;
    
    private ServiceLocator _serviceLocator;
    private NetworkManagerServer _networkManagerServer;
    private NetworkManagerClient _networkManagerClient;

    protected void Start()
    {
        _serviceLocator = ServiceLocator.Global;
        _serviceLocator.Register<ChatScreen>(GetType(), this);
        
        _serviceLocator.Get(out NetworkManagerServer server);
        _networkManagerServer = server;
        
        _serviceLocator.Get(out NetworkManagerClient clietn);
        _networkManagerClient = clietn;
        
        inputMessage.onEndEdit.AddListener(OnEndEdit);

        this.gameObject.SetActive(false);
    }

    public void ReceiveConsoleMessage(string obj)
    {
        messages.text += obj + System.Environment.NewLine;
        Debug.Log(obj);
    }

    void OnEndEdit(string str)
    {
        if (inputMessage.text != "")
        {
            NetConsole temp = new NetConsole(inputMessage.text);
            if (this == _networkManagerServer)
            {
                _networkManagerServer.Broadcast(temp.Serialize());
            }
            else
                _networkManagerClient.SendToServer(temp.Serialize());

            inputMessage.ActivateInputField();
            inputMessage.Select();
            inputMessage.text = "";
        }

    }

}
