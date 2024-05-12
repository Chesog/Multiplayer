      using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class ChatScreen : MonoBehaviour
{
    public Text messages;
    public InputField inputMessage;
    
    private ServiceLocator _serviceLocator;
    private NetworkManager _networkManager;

    protected void Awake()
    {
        _serviceLocator = ServiceLocator.global;
        _serviceLocator.Register<ChatScreen>(GetType(), this);
        _serviceLocator.Get(out NetworkManager manager);
        _networkManager = manager;
        
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
            if (_networkManager.isServer)
            {
                _networkManager.Broadcast(temp.Serialize());
                messages.text += inputMessage.text + System.Environment.NewLine;
            }
            else
                _networkManager.SendToServer(temp.Serialize());

            inputMessage.ActivateInputField();
            inputMessage.Select();
            inputMessage.text = "";
        }

    }

}
