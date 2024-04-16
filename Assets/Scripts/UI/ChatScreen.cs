      using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class ChatScreen : MonoBehaviourSingleton<ChatScreen>
{
    public Text messages;
    public InputField inputMessage;

    protected override void Initialize()
    {
        inputMessage.onEndEdit.AddListener(OnEndEdit);

        this.gameObject.SetActive(false);

        NetworkManager.Instance.OnReceiveEvent += OnReceiveDataEvent;
    }

    private void OnRecieveHandShakeMessage(byte[] obj)
    {
        throw new System.NotImplementedException();
    }

    private void OnRecievePositionMessage(Vector3 obj)
    {
        throw new System.NotImplementedException();
    }

    private void OnRecieveConsoleMessage(string obj)
    {
        messages.text += obj + System.Environment.NewLine;
        Debug.Log(obj);
    }

    void OnReceiveDataEvent(byte[] data, IPEndPoint ep)
    {
        if (NetworkManager.Instance.isServer)
        {
            NetworkManager.Instance.Broadcast(data);
        }
        
        MessageController.Instance.HandleMessage(data);

       // messages.text += System.Text.ASCIIEncoding.UTF8.GetString(data) + System.Environment.NewLine;
    }
    
    

    void OnEndEdit(string str)
    {
        if (inputMessage.text != "")
        {
            if (NetworkManager.Instance.isServer)
            {
                //NetworkManager.Instance.Broadcast(System.Text.ASCIIEncoding.UTF8.GetBytes(inputMessage.text));
                NetConsole temp = new NetConsole(inputMessage.text);
                NetworkManager.Instance.Broadcast(temp.Serialize());
                messages.text += inputMessage.text + System.Environment.NewLine;
            }
            else
            {
                NetConsole temp = new NetConsole(inputMessage.text);
                NetworkManager.Instance.SendToServer(temp.Serialize());
                //NetworkManager.Instance.SendToServer(System.Text.ASCIIEncoding.UTF8.GetBytes(inputMessage.text));
            }

            inputMessage.ActivateInputField();
            inputMessage.Select();
            inputMessage.text = "";
        }

    }

}
